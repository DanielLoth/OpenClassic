using System;
using System.Diagnostics;
using System.Text;

namespace OpenClassic.Server.Networking.Rscd
{
    public class RscdPacketWriter : AbstractPacketWriter
    {
        public void SendServerInfo(ISession session)
        {
            Debug.Assert(session != null);

            CreatePacket(session, 110);
            WriteLong(session, DateTime.Now.Ticks);
            WriteBytes(session, Encoding.UTF8.GetBytes("Australia"));
            FormatPacket(session);
        }

        public void SendFatigue(ISession session)
        {
            Debug.Assert(session != null);

            CreatePacket(session, 126);
            WriteShort(session, 50);
            FormatPacket(session);
        }

        public void SendWorldInfo(ISession session)
        {
            Debug.Assert(session != null);

            CreatePacket(session, 131);

            const int playerIndex = 10;
            WriteShort(session, playerIndex);
            WriteShort(session, 2304);
            WriteShort(session, 1776);
            WriteShort(session, 0);
            WriteShort(session, 944);
            FormatPacket(session);
        }

        public void SendStats(ISession session)
        {
            Debug.Assert(session != null);

            CreatePacket(session, 180);

            const int statCount = 18;

            for (var i = 0; i < statCount; i++)
            {
                const int currentLevel = 50;
                WriteByte(session, currentLevel);
            }

            for (var i = 0; i < statCount; i++)
            {
                const int baseLevel = 50;
                WriteByte(session, baseLevel);
            }

            for (var i = 0; i < statCount; i++)
            {
                const int experience = 100000;
                WriteInt(session, experience);
            }

            FormatPacket(session);
        }

        public void SendLoginBox(ISession session)
        {
            Debug.Assert(session != null);

            CreatePacket(session, 248);

            const int daysSinceLastLogin = 180;
            WriteShort(session, daysSinceLastLogin);

            const int subscriptionDaysLeft = 30;
            WriteShort(session, subscriptionDaysLeft);

            WriteBytes(session, Encoding.UTF8.GetBytes("127.0.0.1"));

            FormatPacket(session);
        }

        public void SendCantLogout(ISession session)
        {
            Debug.Assert(session != null);

            CreatePacket(session, 136);
            FormatPacket(session);
        }
    }
}
