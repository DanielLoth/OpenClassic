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
            var reconnecting = buffer.ReadBoolean();
            var version = buffer.ReadUnsignedShort();

            // Uncomment these once RSA decryption actually works.
            //var loginPacketSize = buffer.ReadByte();
            //var loginBuffer = DecryptRsa(buffer, loginPacketSize);
            var loginBuffer = buffer;

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
            session.Buffer.WriteByte(0);

            if (newPlayer == null)
            {
                const int serverFullResponseCode = 5;
                session.Buffer.WriteByte(serverFullResponseCode);
                session.WriteFlushClose();
                return;
            }
            else
            {
                const int successResponseCode = 0;
                session.Buffer.WriteByte(successResponseCode);

                session.Player = newPlayer;

                packetWriter.SendServerInfo(session);
                packetWriter.SendFatigue(session);
                packetWriter.SendWorldInfo(session);
                packetWriter.SendStats(session);
                packetWriter.SendLoginBox(session);

                session.WriteAndFlushSessionBuffer();
                return;
            }
        }

        private readonly BigInteger _key = BigInteger.Parse(
            "730546719878348732291497161314617369560443701473303681965331739205703475535302276087891130348991033265134162275669215460061940182844329219743687403068279");

        private readonly BigInteger _modulus = BigInteger.Parse(
            "1549611057746979844352781944553705273443228154042066840514290174539588436243191882510185738846985723357723362764835928526260868977814405651690121789896823");


        private IByteBuffer DecryptRsa2(IByteBuffer buffer, int rsaBlockSize)
        {
            Debug.Assert(buffer != null);
            Debug.Assert(rsaBlockSize >= 64);

            //var encryptedBigInt = new BigInteger(encrypted.Reverse().ToArray());
            //var decrypted = BigInteger.ModPow(encryptedBigInt, rsaPrivateKey, rsaModulus).ToByteArray().Reverse().ToArray();

            //return Unpooled.WrappedBuffer(decrypted);

            var encrypted = new byte[rsaBlockSize];
            buffer.ReadBytes(encrypted, 0, rsaBlockSize);

            Debug.Assert(buffer.ReadableBytes == 0);

            var encryptedBigInt = new BigInteger(encrypted.Reverse().ToArray());
            var decrypted = BigInteger.ModPow(encryptedBigInt, _key, _modulus).ToByteArray().Reverse().ToArray();

            return Unpooled.WrappedBuffer(decrypted);
        }

        private IByteBuffer DecryptRsa(IByteBuffer buffer, int rsaBlockSize)
        {
            var expectedDataAfterEncryption = new byte[] { 25, 224, 87, 217, 170, 173, 206, 249, 83, 111, 67, 199, 213, 60, 146, 29, 210, 242, 185, 176, 51, 104, 71, 61, 208, 167, 73, 234, 18, 161, 110, 5, 11, 176, 245, 138, 56, 189, 141, 207, 251, 182, 241, 186, 247, 133, 198, 204, 53, 201, 110, 234, 117, 12, 33, 163, 187, 214, 5, 95, 240, 150, 167, 158 };

            var encrypted = expectedDataAfterEncryption;//new byte[rsaBlockSize];
            buffer.ReadBytes(encrypted, 0, rsaBlockSize);

            var encryptedBigInt = new BigInteger(encrypted.Reverse().ToArray());
            var decrypted = BigInteger.ModPow(encryptedBigInt, _key, _modulus).ToByteArray().Reverse().ToArray();

            return Unpooled.WrappedBuffer(decrypted);
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
}
