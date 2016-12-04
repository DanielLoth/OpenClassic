using DotNetty.Buffers;
using DotNetty.Transport.Channels.Embedded;
using OpenClassic.Server.Networking;
using System;
using Xunit;

namespace OpenClassic.Server.Tests.Networking
{
    public class GameMessageDecoderTests
    {
        private byte[] RequestSession = { 2, 55, 32 };
        private byte[] SendPrivacySetings = { 5, 4, 64, 1, 2, 3 };

        [Fact]
        public void OnePacketSingleByteLen()
        {
            var channel = new EmbeddedChannel(new GameMessageDecoder());
            var buffer = Unpooled.WrappedBuffer(RequestSession);

            channel.WriteInbound(buffer);

            var result = channel.ReadInbound<IByteBuffer>();
            var result2 = channel.ReadInbound<object>(); // Returns null, as there's no second object returned in the List<object>.
        }

        [Fact]
        public void DecodesMultiplePacketsWith1ByteLenInOneInvocation()
        {
            var channel = new EmbeddedChannel(new GameMessageDecoder());
            var buffer = Unpooled.Buffer();
            const int packetCount = 10;

            for (var i = 0; i < packetCount; i++)
            {
                buffer.WriteBytes(RequestSession);
            }

            channel.WriteInbound(buffer);

            for (var i = 0; i < packetCount; i++)
            {
                var packet = channel.ReadInbound<IByteBuffer>();

                Assert.NotNull(packet);
                Assert.Equal(1, packet.ReaderIndex);
                Assert.Equal(1, packet.ReadableBytes);
                Assert.Equal(1, packet.GetPayloadLength());
                Assert.Equal(32, packet.GetOpcode());
                Assert.Equal(55, packet.ReadByte());
                Assert.True(packet.ReferenceCount > 1);
            }

            // Verify that there are no more results (there shouldn't be at this point).
            Assert.Null(channel.ReadInbound<IByteBuffer>());

            Assert.Equal(packetCount, buffer.ReferenceCount);
        }

        private static IByteBuffer GetPaddedByteBuffer(byte[] contents)
        {
            if (contents == null) throw new ArgumentNullException();

            var buffer = GamePooledByteBufferAllocator.Default.Buffer(contents.Length);
            buffer.WriteBytes(contents);

            return buffer;
        }
    }
}
