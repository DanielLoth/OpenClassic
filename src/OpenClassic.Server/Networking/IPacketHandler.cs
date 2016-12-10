using DotNetty.Buffers;

namespace OpenClassic.Server.Networking
{
    public interface IPacketHandler
    {
        int Opcode { get; }
        void Handle(ISession session, IByteBuffer packet);
    }
}
