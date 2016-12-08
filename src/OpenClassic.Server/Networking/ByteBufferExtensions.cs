using DotNetty.Buffers;
using System.Diagnostics;

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
    }
}
