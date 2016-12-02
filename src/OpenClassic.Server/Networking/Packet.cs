using DotNetty.Buffers;
using System.Diagnostics;

namespace OpenClassic.Server.Networking
{
    public struct Packet
    {
        public byte Opcode { get; }
        public IByteBuffer Buffer { get; }

        public Packet(byte opcode, IByteBuffer buffer)
        {
            Debug.Assert(buffer != null);

            Opcode = opcode;
            Buffer = buffer;
        }
    }
}
