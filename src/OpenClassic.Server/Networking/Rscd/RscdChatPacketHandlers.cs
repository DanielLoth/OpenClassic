using DotNetty.Buffers;
using System.Diagnostics;

namespace OpenClassic.Server.Networking.Rscd
{
    internal class AddFriendPacketHandler : IRscdPacketHandlerMarker
    {
        public int Opcode => 168;

        public void Handle(ISession session, IByteBuffer packet)
        {
            Debug.Assert(session != null);
            Debug.Assert(packet != null);
        }
    }

    internal class RemoveFriendPacketHandler : IRscdPacketHandlerMarker
    {
        public int Opcode => 52;

        public void Handle(ISession session, IByteBuffer packet)
        {
            Debug.Assert(session != null);
            Debug.Assert(packet != null);
        }
    }

    internal class AddIgnorePacketHandler : IRscdPacketHandlerMarker
    {
        public int Opcode => 25;

        public void Handle(ISession session, IByteBuffer packet)
        {
            Debug.Assert(session != null);
            Debug.Assert(packet != null);
        }
    }

    internal class RemoveIgnorePacketHandler : IRscdPacketHandlerMarker
    {
        public int Opcode => 108;

        public void Handle(ISession session, IByteBuffer packet)
        {
            Debug.Assert(session != null);
            Debug.Assert(packet != null);
        }
    }

    internal class PublicChatPacketHandler : IRscdPacketHandlerMarker
    {
        public int Opcode => 145;

        public void Handle(ISession session, IByteBuffer packet)
        {
            Debug.Assert(session != null);
            Debug.Assert(packet != null);
        }
    }
}
