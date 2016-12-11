using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using DryIoc;
using OpenClassic.Server.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace OpenClassic.Server.Networking
{
    public class GameConnectionHandler : ChannelHandlerAdapter, ISession
    {
        private static readonly IGameEngine gameEngine;
        private static readonly IPacketHandler[] PacketHandlerMap;

        private readonly IChannel gameChannel;

        public override bool IsSharable => false;

        public IChannel ClientChannel => gameChannel;

        // This needs to be volatile because this field can be set by either
        // a worker thread OR the game thread.
        private volatile IByteBuffer _buffer;

        public bool AllowedToDisconnect => false;

        public IByteBuffer Buffer => _buffer;

        public int CurrentPacketStartIndex { get; set; }

        public int? CurrentPacketId { get; set; }

        public int CurrentPacketBitfieldPosition { get; set; }

        public int MaxPacketLength => _buffer?.Capacity ?? 0;

        #region Packet queue fields and properties

        private readonly List<IByteBuffer> packetQueueOne = new List<IByteBuffer>(128);
        private readonly List<IByteBuffer> packetQueueTwo = new List<IByteBuffer>(128);
        private readonly object packetQueueLock = new object();

        private List<IByteBuffer> CurrentPacketQueue { get; set; }

        #endregion

        public static void Init()
        {
            // Do nothing - this is just to invoke the static 
            // GameConnectionHandler() constructor.
        }

        static GameConnectionHandler()
        {
            var resolver = DependencyResolver.Current;

            gameEngine = resolver.Resolve<IGameEngine>();

            // This needs to be executed on the game thread. Otherwise the
            // game thread won't necessarily see the loaded IPacketHandler map
            // when it invokes the Pulse() method each game tick.
            Debug.Assert(gameEngine.IsOnGameThread);

            var packetHandlers = resolver.Resolve<IPacketHandler[]>();
            var handlerMap = new IPacketHandler[255];

            foreach (var handler in packetHandlers)
            {
                handlerMap[handler.Opcode] = handler;
            }

            for (var i = 0; i < handlerMap.Length; i++)
            {
                if (handlerMap[i] == null)
                {
                    handlerMap[i] = new NoOpPacketHandler();
                }
            }

            PacketHandlerMap = handlerMap;
        }

        public GameConnectionHandler(IChannel channel)
        {
            Debug.Assert(channel != null);

            this.gameChannel = channel;

            AllocateBuffer();
            Debug.Assert(_buffer != null);
            Debug.Assert(_buffer.ReferenceCount == 1);

            CurrentPacketQueue = packetQueueOne;
        }

        public override void ChannelInactive(IChannelHandlerContext context)
        {
            base.ChannelInactive(context);
        }

        public override void ChannelActive(IChannelHandlerContext context)
        {
            Debug.Assert(context != null);

            var channel = context.Channel;

            base.ChannelActive(context);
        }

        public override void ChannelRead(IChannelHandlerContext context, object message)
        {
            Debug.Assert(context != null);
            Debug.Assert(message != null);
            Debug.Assert(message is IByteBuffer);

            var buffer = message as IByteBuffer;

            // Add the message to the queue. Use AddMessage(IByteBuffer) so that
            // the message is added while the packetQueueLock is held.
            AddMessageThreadSafe(buffer);
        }

        public int Pulse()
        {
            // Pulse must only ever be invoked on the game thread.
            Debug.Assert(gameEngine.IsOnGameThread);

            // Get the list of messages requiring processing. Make use of the
            // GetAndSwapPacketQueueThreadSafe() method to ensure that the
            // correct lock is held while retrieving queued packets.
            var messages = GetAndSwapPacketQueueThreadSafe();

            try
            {
                var packetsHandled = 0;

                foreach (var buffer in messages)
                {
                    try
                    {
                        var opcode = buffer.GetOpcode();
                        var handler = PacketHandlerMap[opcode];

                        handler.Handle(this, buffer);

                        packetsHandled++;
                    }
#pragma warning disable RECS0022 // A catch clause that catches System.Exception and has an empty body
                    catch (Exception ex)
#pragma warning restore RECS0022 // A catch clause that catches System.Exception and has an empty body
                    {
                        var inner = ex.InnerException;
                        // TODO: Determine what we want to do on packet handler failure.
                    }
                    finally
                    {
                        buffer.Release();
                    }
                }

                return packetsHandled;
            }
            finally
            {
                // We need to lock again to ensure that other threads accessing the packet queue
                // in the future see the correctly-cleared queue.
                lock (packetQueueLock)
                {
                    messages.Clear();

                    Debug.Assert(messages.Count == 0);
                    Debug.Assert(messages != CurrentPacketQueue);
                }
            }
        }

        #region Message queueing

        private void AddMessageThreadSafe(IByteBuffer message)
        {
            // This method should only ever be called by DotNetty worker threads.
            Debug.Assert(!gameEngine.IsOnGameThread);

            Debug.Assert(message != null);

            // We need to lock here to guarantee memory visibility of the queue contents.
            lock (packetQueueLock)
            {
                var currentQueue = CurrentPacketQueue;

                Debug.Assert(currentQueue != null);

                currentQueue.Add(message);
            }
        }

        private List<IByteBuffer> GetAndSwapPacketQueueThreadSafe()
        {
            List<IByteBuffer> messages = null;

            // We need to lock here to guarantee memory visibility.
            lock (packetQueueLock)
            {
                var currentQueue = CurrentPacketQueue;
                Debug.Assert(currentQueue != null);

                var otherQueue = ReferenceEquals(currentQueue, packetQueueTwo) ? packetQueueOne : packetQueueTwo;

                Debug.Assert(otherQueue != null);
                Debug.Assert(otherQueue.Count == 0); // The other queue should currently be empty.

                // Assign the current queue to our messages variable so that we can iterate
                // over it in a moment once we're outside of this mutex.
                messages = currentQueue;

                // Finally, also swap the queues over while inside the mutex.
                CurrentPacketQueue = otherQueue;

                Debug.Assert(messages != null);
                Debug.Assert(messages != CurrentPacketQueue);
            }

            return messages;
        }

        #endregion

        public Task WriteAndFlushSessionBuffer()
        {
            // Invocation of this method should be on the game thread.
            Debug.Assert(gameEngine.IsOnGameThread);

            var bufferBeforeFlush = _buffer;
            Debug.Assert(bufferBeforeFlush != null);
            Debug.Assert(bufferBeforeFlush.ReferenceCount == 1);

            var writeAndFlushResult = gameChannel.WriteAndFlushAsync(bufferBeforeFlush);

            AllocateBuffer();

            var bufferAfterFlush = _buffer;
            Debug.Assert(bufferAfterFlush != null);
            Debug.Assert(bufferBeforeFlush != bufferAfterFlush);
            Debug.Assert(bufferAfterFlush.ReferenceCount == 1);

            return writeAndFlushResult;
        }

        public Task WriteFlushClose()
        {
            // Invocation of this method should be on the game thread.
            Debug.Assert(gameEngine.IsOnGameThread);

            Debug.Assert(_buffer != null);
            Debug.Assert(_buffer.ReferenceCount == 1);

            var writeFinishedFuture = gameChannel.WriteAndFlushAsync(_buffer);

            var unregisterAndCloseOnGameEngineThread = writeFinishedFuture.ContinueWith(async (t) =>
            {
                // This callback is running on a worker thread.
                Debug.Assert(!gameEngine.IsOnGameThread);

                // Worker thread awaits channel closure.
                await gameChannel.CloseAsync();

                // And now that the channel is closed, unregister the session
                // in the GameEngine.
                gameEngine.UnregisterSession(this);
            });

            return unregisterAndCloseOnGameEngineThread;
        }

        private void AllocateBuffer()
        {
            // This method could be called by either thread, which is why
            // the _buffer field is volatile.

            _buffer = gameChannel.Allocator.Buffer();
        }
    }
}
