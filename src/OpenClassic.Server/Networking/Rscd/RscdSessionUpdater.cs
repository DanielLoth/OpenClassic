using System.Diagnostics;

namespace OpenClassic.Server.Networking.Rscd
{
    public class RscdSessionUpdater : ISessionUpdater
    {
        private readonly RscdPacketWriter packetWriter;

        public RscdSessionUpdater(RscdPacketWriter packetWriter)
        {
            Debug.Assert(packetWriter != null);

            this.packetWriter = packetWriter;
        }

        public void Update(ISession session)
        {
            Debug.Assert(session != null);

            packetWriter.SendPlayerPositionUpdate(session);
            packetWriter.SendNpcPositionUpdate(session);
            packetWriter.SendGameObjectUpdate(session);
            packetWriter.SendWallObjectUpdate(session);
            packetWriter.SendItemUpdate(session);

            packetWriter.SendPlayerAppearanceUpdate(session);
            packetWriter.SendNpcAppearanceUpdate(session);

            session.WriteAndFlushSessionBuffer();
        }
    }
}
