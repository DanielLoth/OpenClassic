using DotNetty.Buffers;
using OpenClassic.Server.Configuration;
using OpenClassic.Server.Domain;
using System;
using System.Diagnostics;
using System.Linq;
using System.Numerics;

namespace OpenClassic.Server.Networking.Rscd
{
    internal class SessionRequestMessageHandler : IRscdPacketHandlerMarker
    {
        public int Opcode => 32;

        private readonly RscdPacketWriter packetWriter;

        public SessionRequestMessageHandler(RscdPacketWriter packetWriter)
        {
            Debug.Assert(packetWriter != null);

            this.packetWriter = packetWriter;
        }

        public void Handle(ISession session, IByteBuffer packet)
        {
            Debug.Assert(session != null);
            Debug.Assert(packet != null);

            var userByte = packet.ReadByte();

            session.Buffer.WriteLong(1337);
            session.WriteAndFlushSessionBuffer();
        }
    }

    internal class LoginMessageHandler : IRscdPacketHandlerMarker
    {
        public int Opcode => 0;

        private readonly IConfig config;
        private readonly IGameEngine engine;
        private readonly IWorld world;
        private readonly BigInteger rsaPrivateKey;
        private readonly BigInteger rsaModulus;
        private readonly RscdPacketWriter packetWriter;

        public LoginMessageHandler(IConfig config, IGameEngine engine, IWorld world, RscdPacketWriter packetWriter)
        {
            Debug.Assert(config != null);
            Debug.Assert(engine != null);
            Debug.Assert(world != null);
            Debug.Assert(packetWriter != null);

            this.config = config;
            this.engine = engine;
            this.world = world;
            this.packetWriter = packetWriter;

            rsaPrivateKey = BigInteger.Parse(config.RsaDecryptionKey);
            rsaModulus = BigInteger.Parse(config.RsaModulus);
        }

        public void Handle(ISession session, IByteBuffer packet)
        {
            Debug.Assert(session != null);
            Debug.Assert(packet != null);

            var buffer = packet;
            buffer.MarkReaderIndex();
            var bytes = new byte[buffer.ReadableBytes];
            buffer.ReadBytes(bytes, 0, bytes.Length);
            buffer.ResetReaderIndex();

            var reconnecting = buffer.ReadBoolean();
            var version = buffer.ReadUnsignedShort();

            // Uncomment these once RSA decryption actually works.
            var loginPacketSize = buffer.ReadByte();
            var loginBuffer = DecryptRsa(buffer, loginPacketSize);

            var sessionKeys = new int[4];
            for (var i = 0; i < sessionKeys.Length; i++)
            {
                sessionKeys[i] = loginBuffer.ReadInt();
            }

            var uid = loginBuffer.ReadInt();
            var username = loginBuffer.ReadString(20).Trim();
            var password = loginBuffer.ReadString(20).Trim();

            Console.WriteLine($"Login: {uid} - {username}:{password}");

            var newPlayer = world.GetAvailablePlayer();
            if (newPlayer == null)
            {
                session.Buffer.WriteByte(5); // Rejected login.
                session.WriteFlushClose();
                return;
            }
            else
            {
                newPlayer.Location = Player.DEFAULT_LOCATION;

                session.Player = newPlayer;

                session.Buffer.WriteByte(0); // Successful login.

                packetWriter.SendServerInfo(session);
                packetWriter.SendFatigue(session);
                packetWriter.SendWorldInfo(session);
                packetWriter.SendStats(session);
                packetWriter.SendLoginBox(session);

                packetWriter.SendPlayerPositionUpdate(session);

                packetWriter.SendInventory(session);
                packetWriter.SendCombatStyle(session);
                packetWriter.SendClientConfig(session);

                //packetWriter.SendBank(session);

                //packetWriter.SendShowAppearanceScreen(session);
                //packetWriter.SendDied(session);

                session.WriteAndFlushSessionBuffer();
                return;
            }
        }

        private IByteBuffer DecryptRsa(IByteBuffer buffer, int rsaBlockSize)
        {
            Debug.Assert(buffer != null);
            Debug.Assert(rsaBlockSize == 64);
            Debug.Assert(buffer.ReadableBytes == rsaBlockSize);

            var encryptedPayload = new byte[rsaBlockSize];
            buffer.ReadBytes(encryptedPayload, 0, rsaBlockSize);

            var encryptedBigInt = new BigInteger(encryptedPayload.Reverse().ToArray());

            var decrypted = BigInteger.ModPow(encryptedBigInt, rsaPrivateKey, rsaModulus).ToByteArray().Reverse().ToArray();
            var result = Unpooled.WrappedBuffer(decrypted);

            return result;
        }
    }

    internal class PingMessageHandler : IRscdPacketHandlerMarker
    {
        public int Opcode => 5;

        public void Handle(ISession session, IByteBuffer packet)
        {
            Debug.Assert(session != null);
            Debug.Assert(packet != null);
            Debug.Assert(packet.ReadableBytes == 0);
        }
    }

    internal class LogoutRequestMessageHandler : IRscdPacketHandlerMarker
    {
        public int Opcode => 129;

        private readonly RscdPacketWriter packetWriter;

        public LogoutRequestMessageHandler(RscdPacketWriter packetWriter)
        {
            Debug.Assert(packetWriter != null);

            this.packetWriter = packetWriter;
        }

        public void Handle(ISession session, IByteBuffer packet)
        {
            Debug.Assert(session != null);
            Debug.Assert(packet != null);
            Debug.Assert(packet.ReadableBytes == 0);

            packetWriter.SendLogout(session);
            session.WriteAndFlushSessionBuffer();
        }
    }

    internal class LogoutMessageHandler : IRscdPacketHandlerMarker
    {
        public int Opcode => 39;

        private readonly RscdPacketWriter packetWriter;

        public LogoutMessageHandler(RscdPacketWriter packetWriter)
        {
            Debug.Assert(packetWriter != null);

            this.packetWriter = packetWriter;
        }

        public void Handle(ISession session, IByteBuffer packet)
        {
            Debug.Assert(session != null);
            Debug.Assert(packet != null);
            Debug.Assert(packet.ReadableBytes == 0);

            packetWriter.SendLogout(session);
            session.WriteAndFlushSessionBuffer();
        }
    }
}
