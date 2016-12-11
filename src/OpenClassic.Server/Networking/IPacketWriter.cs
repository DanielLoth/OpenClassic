namespace OpenClassic.Server.Networking
{
    public interface IPacketWriter
    {
        void CreatePacket(ISession session, int id);
        void FormatPacket(ISession session);
        void WriteBits(ISession session, int value, int numberOfBits);
        void WriteByte(ISession session, int value);
        void WriteBytes(ISession session, byte[] src);
        void WriteBytes(ISession session, byte[] src, int srcIndex, int length);
        void WriteShort(ISession session, int value);
        void WriteInt(ISession session, int value);
        void WriteLong(ISession session, long value);
        void SendServerInfo(ISession session);
        void SendFatigue(ISession session);
        void SendWorldInfo(ISession session);
        void SendStats(ISession session);
        void SendLoginBox(ISession session);
    }
}
