using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace OpenClassic.Server.Networking.Nu
{
    public class Session : IDisposable
    {
        private static readonly EventHandler<SocketAsyncEventArgs> SendCompletionEventHandler = new EventHandler<SocketAsyncEventArgs>(SendCompletion);
        private static readonly EventHandler<SocketAsyncEventArgs> ReceiveCompletionEventHandler = new EventHandler<SocketAsyncEventArgs>(ReceiveCompletion);
        private static readonly int BufferLength = 8192;

        public Socket Socket { get; internal set; }

        private readonly SocketAsyncEventArgs receiveArgs = new SocketAsyncEventArgs();
        private readonly ArraySegment<byte> receiveBuffer;
        private int readIndex = 0;
        private int nextPacketStartIndex = 0;

        private readonly SocketAsyncEventArgs sendArgs = new SocketAsyncEventArgs();
        private readonly ArraySegment<byte> sendBuffer;

        private bool disposing = false;

        private void CloseClientSocket()
        {
            try
            {
                Socket.Shutdown(SocketShutdown.Both);
            }
            catch (Exception) { }

            Console.WriteLine("A client has been disconnected from the server.");
        }

        public Session()
        {
            // Set up the read buffer. In the future, we'll use a slice from a much larger byte array.
            {
                var rBuf = new byte[BufferLength];
                receiveBuffer = new ArraySegment<byte>(rBuf, 0, rBuf.Length);

                receiveArgs.SetBuffer(receiveBuffer.Array, receiveBuffer.Offset, receiveBuffer.Count);
                receiveArgs.UserToken = this;
                receiveArgs.Completed += ReceiveCompletionEventHandler;
            }

            // Set up the write buffer. Same deal - use a slice from large byte array in the future.
            {
                var sBuf = new byte[BufferLength];
                sendBuffer = new ArraySegment<byte>(sBuf, 0, sBuf.Length);

                sendArgs.SetBuffer(sendBuffer.Array, sendBuffer.Offset, sendBuffer.Count);
                sendArgs.UserToken = this;
                sendArgs.Completed += SendCompletionEventHandler;
            }

#if DEBUG
            // Postconditions
            Debug.Assert(receiveBuffer.Array != null);
            Debug.Assert(receiveBuffer.Offset >= 0);
            Debug.Assert(receiveBuffer.Offset < receiveBuffer.Array.Length);
            Debug.Assert(receiveBuffer.Count > 0);

            Debug.Assert(sendBuffer.Array != null);
            Debug.Assert(sendBuffer.Offset >= 0);
            Debug.Assert(sendBuffer.Offset < sendBuffer.Array.Length);
            Debug.Assert(sendBuffer.Count > 0);

            Debug.Assert(receiveArgs.UserToken != null);

            Debug.Assert(sendArgs.UserToken != null);
#endif
        }

        public void Start()
        {
            ReceiveStart();
        }

        #region Receive data

        private void ReceiveStart()
        {
            try
            {
                var willRaiseEvent = false;

                do
                {
                    var args = receiveArgs;
                    willRaiseEvent = Socket.ReceiveAsync(args);

                    if (!willRaiseEvent)
                    {
                        ReceiveProcess(args);
                    }
                } while (!willRaiseEvent);
            }
            catch
            {
                Dispose(false);
            }
        }

        private static void ReceiveCompletion(object sender, SocketAsyncEventArgs e)
        {
            ReceiveProcess(e);

            var session = (Session)e.UserToken;
            if (!session.disposing)
            {
                session.ReceiveStart();
            }
        }

        private static void MoveBufferContentsToStartOf(ArraySegment<byte> segment, int srcOffset)
        {
#if DEBUG
            // Preconditions
            Debug.Assert(segment.Array != null);
            Debug.Assert(segment.Count > 0);
            Debug.Assert(segment.Offset >= 0);
            Debug.Assert(segment.Offset < segment.Count);

            Debug.Assert(srcOffset >= 0);
            Debug.Assert(srcOffset < segment.Count);
#endif
            var startIndex = segment.Offset;
            var actualSrcOffset = startIndex + srcOffset;
            var bytesToMove = segment.Count - srcOffset;
            var buffer = segment.Array;

            // Do the copy
            Buffer.BlockCopy(buffer, actualSrcOffset, buffer, 0, bytesToMove);
        }

        private struct Packet
        {
            private readonly int id;
            private readonly byte[] payload;

            public int Id => id;
            public byte[] Payload => payload;

            public Packet(int id, byte[] payload)
            {
                this.id = id;
                this.payload = payload;
            }
        }

        private static void ReceiveProcess(SocketAsyncEventArgs e)
        {
#if DEBUG
            // Preconditions
            Debug.Assert(e != null);
            Debug.Assert(e.UserToken != null);
            Debug.Assert(e.Buffer != null);
#endif
            var session = (Session)e.UserToken;
            if (session.disposing)
            {
                return;
            }

            var bytesTransferred = e.BytesTransferred;
            if (bytesTransferred == 0 || e.SocketError != SocketError.Success)
            {                // Socket closed, or error occurred.

                session.Dispose(false);
                return;
            }

            var packetStartIndex = session.nextPacketStartIndex;
            var lastIndex = session.readIndex + bytesTransferred;
            var currentReadIndex = packetStartIndex;
            var buffer = e.Buffer;
            var availableBytes = lastIndex - packetStartIndex;
            var packets = new List<Packet>();

            do
            {
                if (availableBytes < 2)
                {
                    break; // TODO: Handle insufficient data.
                }

                var firstByte = buffer[currentReadIndex++];
                var payloadLen = 0;

                if (firstByte >= 160)
                {
                    payloadLen = (firstByte - 160) * 256 + buffer[currentReadIndex++];
                    availableBytes -= 2;
                }
                else
                {
                    payloadLen = firstByte;
                    --availableBytes;
                }

                // If we have a full packet ready to read.
                if (payloadLen <= availableBytes)
                {
                    byte lastByte = 0;
                    if (payloadLen < 160 && payloadLen > 1)
                    {
                        lastByte = buffer[currentReadIndex++];
                        --availableBytes;
                        --payloadLen;
                    }

                    var payload = new byte[payloadLen];

                    var id = buffer[currentReadIndex++];
                    payloadLen--;
                    availableBytes--;

                    if (payloadLen >= 160 || payloadLen > 0)
                    {
                        Array.Copy(buffer, currentReadIndex, payload, 0, payload.Length);
                        currentReadIndex += payload.Length;
                        availableBytes -= payload.Length;
                    }

                    if (payloadLen < 160)
                    {
                        payload[payloadLen] = lastByte;
                    }

                    var packet = new Packet(id, payload);
                    packets.Add(packet);
                }
                else
                {
                    break; // No full packet remaining
                }

            } while (availableBytes > 0);

            // Down here, we set up properly for the next time we receive data.

#if DEBUG
            // Postconditions
#endif
        }

        #endregion

        #region Send data

        private void SendStart()
        {
            try
            {
                var willRaiseEvent = false;

                do
                {
                    var args = sendArgs;

                    willRaiseEvent = Socket.SendAsync(args);

                    if (!willRaiseEvent)
                    {
                        SendProcess(args);
                    }
                } while (!willRaiseEvent);
            }
            catch
            {
                Dispose(false);
            }
        }

        private static void SendCompletion(object sender, SocketAsyncEventArgs e)
        {
            SendProcess(e);

            var session = (Session)e.UserToken;
            if (session.disposing)
            {
                return;
            }
        }

        private static void SendProcess(SocketAsyncEventArgs e)
        {
            var session = (Session)e.UserToken;
            if (e.SocketError != SocketError.Success || e.BytesTransferred <= 0)
            {
                session.Dispose(false);
                return;
            }
        }

        #endregion

        #region IDisposable members

        public void Dispose()
        {
            Dispose(true);
        }

        public void Dispose(bool flush)
        {
            if (disposing)
            {
                return;
            }

            disposing = true;

            CloseClientSocket();

            Socket.Dispose();
            Socket = null;
        }

        #endregion
    }
}
