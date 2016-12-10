using DotNetty.Buffers;

namespace OpenClassic.Server.Networking
{
    public class NoOpPacketHandler : IPacketHandler
    {
        public int Opcode => -1;

        public void Handle(ISession session, IByteBuffer packet) { }
    }
}
