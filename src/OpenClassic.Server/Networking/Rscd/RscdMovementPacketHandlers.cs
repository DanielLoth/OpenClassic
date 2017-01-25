using DotNetty.Buffers;
using OpenClassic.Server.Domain;
using System.Diagnostics;

namespace OpenClassic.Server.Networking.Rscd
{
    public class RscdWalkPacketHandler : IRscdPacketHandlerMarker
    {
        public int Opcode => 132;

        private readonly RscdPacketWriter _packetWriter;

        private int _counter = 1;

        public RscdWalkPacketHandler(RscdPacketWriter packetWriter)
        {
            Debug.Assert(packetWriter != null);

            _packetWriter = packetWriter;
        }

        public void Handle(ISession session, IByteBuffer packet)
        {
            HandleWalkPacket(session, packet);
            _packetWriter.SendMessage(session, $"Walk message {_counter++} received");
        }

        public static void HandleWalkPacket(ISession session, IByteBuffer packet)
        {
            Debug.Assert(session != null);
            Debug.Assert(packet != null);

            var player = session.Player;
            Debug.Assert(player != null);

            var startX = packet.ReadShort();
            var startY = packet.ReadShort();
            var stepCount = packet.ReadableBytes / 2;

            var xOffsets = new byte[stepCount];
            var yOffsets = new byte[stepCount];

            for (var i = 0; i < stepCount; i++)
            {
                xOffsets[i] = packet.ReadByte();
                yOffsets[i] = packet.ReadByte();
            }

            var path = new Path(startX, startY, xOffsets, yOffsets);
            player.SetPath(path);

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
