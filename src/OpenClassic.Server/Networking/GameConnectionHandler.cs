using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using System.Collections.Generic;
using System.Diagnostics;

namespace OpenClassic.Server.Networking
{
    public class GameConnectionHandler : ChannelHandlerAdapter, ISession
    {
        private readonly IChannel gameChannel;

        public override bool IsSharable => false;

        public IChannel ClientChannel => gameChannel;

        public bool AllowedToDisconnect => false;

        #region Packet queue fields and properties

        private readonly List<IByteBuffer> packetQueueOne = new List<IByteBuffer>(128);
        private readonly List<IByteBuffer> packetQueueTwo = new List<IByteBuffer>(128);
        private readonly object packetQueueLock = new object();

        private List<IByteBuffer> CurrentPacketQueue { get; set; }

        #endregion

        public GameConnectionHandler(IChannel channel)
        {
            Debug.Assert(channel != null);

            this.gameChannel = channel;
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

            base.ChannelRead(context, message);
        }

        public bool Pulse()
        {
            List<IByteBuffer> messages = null;

            try
            {
                // We need to lock here to guarantee memory visibility.
                lock (packetQueueLock)
                {
                    var currentQueue = CurrentPacketQueue;
                    Debug.Assert(currentQueue != null);

                    var otherQueue = ReferenceEquals(currentQueue, packetQueueTwo) ? packetQueueOne : packetQueueTwo;
                    Debug.Assert(otherQueue.Count == 0); // The other queue should currently be empty.

                    // Assign the current queue to our messages variable so that we can iterate
                    // over it in a moment once we're outside of this mutex.
                    messages = currentQueue;

                    // Finally, also swap the queues over while inside the mutex.
                    CurrentPacketQueue = otherQueue;

                    Debug.Assert(messages != null);
                    Debug.Assert(messages != CurrentPacketQueue);
                }

                foreach (var buffer in messages)
                {
                    // TODO: Handle each packet in succession.
                }

                return true;
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

        private void AddMessage(IByteBuffer message)
        {
            Debug.Assert(message != null);

            // We need to lock here to guarantee memory visibility of the queue contents.
            lock (packetQueueLock)
            {
                var currentQueue = CurrentPacketQueue;

                Debug.Assert(currentQueue != null);

                currentQueue.Add(message);
            }
        }

        #endregion
    }
}
