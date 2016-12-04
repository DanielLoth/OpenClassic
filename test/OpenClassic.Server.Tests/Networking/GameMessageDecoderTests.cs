using DotNetty.Buffers;
using DotNetty.Transport.Channels.Embedded;
using OpenClassic.Server.Networking;
using Xunit;

namespace OpenClassic.Server.Tests.Networking
{
    public class GameMessageDecoderTests
    {
        readonly byte[] RequestSession = { 2, 55, 32 };
        readonly byte[] SendPrivacySetings = { 5, 4, 64, 1, 2, 3 };

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
        public void DotNettyDoesNotDecreaseReferenceCount()
        {
            var channel = new EmbeddedChannel(new GameMessageDecoder());
            var buffer = Unpooled.Buffer();

            buffer.WriteBytes(RequestSession);
            var startingRefCount = buffer.ReferenceCount;

            channel.WriteInbound(buffer);
            var result = channel.ReadInbound<IByteBuffer>();

            Assert.NotNull(result);
            Assert.Equal(0, buffer.ReadableBytes);
            Assert.Equal(startingRefCount, result.ReferenceCount);
        }

        [Fact]
        public void DecodesMultiplePacketsWith1ByteLenInOneInvocation()
        {
            var decoder = new GameMessageDecoder();
            var channel = new EmbeddedChannel(decoder);
            var buffer = Unpooled.Buffer();
            const int packetCount = 10;

            for (var i = 0; i < packetCount; i++)
            {
                buffer.WriteBytes(RequestSession);
            }

            channel.WriteInbound(buffer);

            Assert.Equal(packetCount, buffer.ReferenceCount);

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
        }

        [Fact]
        public void DecoderReadsAllBytesFromUnderlyingBuffer()
        {
            var decoder = new GameMessageDecoder();
            var channel = new EmbeddedChannel(decoder);
            var buffer = Unpooled.Buffer();
            const int packetCount = 10;

            for (var i = 0; i < packetCount; i++)
            {
                buffer.WriteBytes(RequestSession);
            }

            channel.WriteInbound(buffer);

            Assert.Equal(0, buffer.ReadableBytes);
        }

        [Fact]
        public void UnderlyingBufferReferenceCountEqualsZeroWhenAllSlicesReleased()
        {
            var decoder = new GameMessageDecoder();
            var channel = new EmbeddedChannel(decoder);
            var buffer = Unpooled.Buffer();
            const int packetCount = 10;

            for (var i = 0; i < packetCount; i++)
            {
                buffer.WriteBytes(RequestSession);
            }

            channel.WriteInbound(buffer);

            Assert.Equal(packetCount, buffer.ReferenceCount);

            for (var i = 0; i < packetCount; i++)
            {
                var packet = channel.ReadInbound<IByteBuffer>();

                packet.Release();
            }

            Assert.Equal(0, buffer.ReferenceCount);
        }
    }
}
