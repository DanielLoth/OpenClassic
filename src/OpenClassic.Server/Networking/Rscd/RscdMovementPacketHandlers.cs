using DotNetty.Buffers;
using System.Diagnostics;

namespace OpenClassic.Server.Networking.Rscd
{
    public class RscdWalkPacketHandler : IRscdPacketHandlerMarker
    {
        public int Opcode => 132;

        public void Handle(ISession session, IByteBuffer packet)
        {
            HandleWalkPacket(session, packet);
        }

        public static void HandleWalkPacket(ISession session, IByteBuffer packet)
        {
            Debug.Assert(session != null);
            Debug.Assert(packet != null);

            var player = session.Player;
            Debug.Assert(player != null);

            player.StartX = packet.ReadShort();
            player.StartY = packet.ReadShort();

            var stepCount = packet.ReadableBytes / 2;

            for (var i = 0; i < stepCount; i++)
            {
                player.XOffsets[i] = packet.ReadByte();
                player.YOffsets[i] = packet.ReadByte();
            }

            player.StepCount = stepCount;

            Debug.Assert(packet.ReadableBytes == 0);
        }
    }

    public class RscdWalkToTargetPacketHandler : IRscdPacketHandlerMarker
    {
        public int Opcode => 246;

        public void Handle(ISession session, IByteBuffer packet)
        {
            RscdWalkPacketHandler.HandleWalkPacket(session, packet);
        }
    }
}
