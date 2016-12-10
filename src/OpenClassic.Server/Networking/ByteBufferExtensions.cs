using DotNetty.Buffers;
using System.Diagnostics;
using System.Text;

namespace OpenClassic.Server.Networking
{
    public static class ByteBufferExtensions
    {
        public static int GetOpcode(this IByteBuffer buffer)
        {
            Debug.Assert(buffer != null);

            return buffer.GetByte(0);
        }

        public static void SetOpcode(this IByteBuffer buffer, int opcode)
        {
            Debug.Assert(buffer != null);

            buffer.SetByte(0, opcode);
        }

        public static int GetPayloadLength(this IByteBuffer buffer)
        {
            Debug.Assert(buffer != null);

            return buffer.Capacity - 1;
        }

        public static string ReadString(this IByteBuffer buffer, int length)
        {
            Debug.Assert(buffer != null);
            Debug.Assert(buffer.ReadableBytes >= length);

            var readable = buffer.ReadableBytes;
            var readIndex = buffer.ReaderIndex;

            var data = new byte[length];
            buffer.ReadBytes(data);

            var result = Encoding.UTF8.GetString(data, 0, data.Length);

            return result;
        }

        public static string ReadString(this IByteBuffer buffer)
        {
            Debug.Assert(buffer != null);

            var startIndex = buffer.ReaderIndex;
            var curByte = 0;

            while (curByte != 10)
            {
                curByte = buffer.ReadByte();
            }

            var endIndex = buffer.ReaderIndex;
            var data = new byte[endIndex - startIndex];

            buffer.SetReaderIndex(startIndex);
            buffer.ReadBytes(data);

            var result = Encoding.UTF8.GetString(data, 0, data.Length);

            return result;
        }
    }
}
