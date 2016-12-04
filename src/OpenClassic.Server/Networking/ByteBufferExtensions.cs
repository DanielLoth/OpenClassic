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

        public static int GetPayloadLength(this IByteBuffer buffer)
        {
            Debug.Assert(buffer != null);

            return buffer.Capacity - 1;
        }
    }
}
