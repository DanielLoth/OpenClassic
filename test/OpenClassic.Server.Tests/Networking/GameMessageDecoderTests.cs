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

        byte[] TwoByteLengthPacket
        {
            get
            {
                byte[] packetBytes = new byte[803];
                for (var i = 0; i < packetBytes.Length; i++)
                {
                    packetBytes[i] = 1;
                }

                packetBytes[0] = 163; // First length byte
                packetBytes[1] = 33; // Second length byte
                packetBytes[2] = 32; // Opcode

                return packetBytes;
            }
        }

        #region 1 byte length

        [Theory]
        [InlineData(new byte[] { 1, 20 })]
        [InlineData(new byte[] { 2, 55, 32 })]
        [InlineData(new byte[] { 2, 22, 77 })]
        [InlineData(new byte[] { 5, 4, 64, 1, 2, 3 })]
        [InlineData(new byte[] { 8, 2, 64, 155, 2, 3, 2, 2, 99 })]
        public void OnePacket1ByteLen(byte[] packetData)
        {
            var channel = new EmbeddedChannel(new GameMessageDecoder());
            var buffer = Unpooled.CopiedBuffer(packetData);

            channel.WriteInbound(buffer);

            var packet = channel.ReadInbound<IByteBuffer>();
            var expectedOpcode = packetData.Length > 2 ? packetData[2] : packetData[1];
            var expectedLength = packetData.Length > 2 ? packetData.Length - 2 : 0;
            
            Assert.NotNull(packet);
            Assert.Equal(1, packet.ReaderIndex);
            Assert.Equal(expectedLength, packet.ReadableBytes);
            Assert.Equal(expectedLength, packet.GetPayloadLength());
            Assert.Equal(expectedOpcode, packet.GetOpcode());

            if (expectedLength > 0)
            {
                var expectedFirstByteRead = expectedLength == 1 ? packetData[1] : packetData[3];

                Assert.Equal(expectedFirstByteRead, packet.ReadByte());
            }

            //Assert.True(packet.ReferenceCount > 1); // Assertion fails - TODO: Why??

            // Returns null, as there's no second object returned in the List<object>.
            Assert.Null(channel.ReadInbound<IByteBuffer>());
        }

        [Fact]
        public void MultiplePackets_1ByteLen_ProcessesMultipleInSingleInvocation()
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

            for (var i = 0; i < packetCount; i++)
            {
                var packet = channel.ReadInbound<IByteBuffer>();

                Assert.NotNull(packet);
                Assert.Equal(1, packet.ReaderIndex);
                Assert.Equal(1, packet.ReadableBytes);
                Assert.Equal(1, packet.GetPayloadLength());
                Assert.Equal(32, packet.GetOpcode());
                Assert.Equal(55, packet.ReadByte());
                Assert.Equal(packetCount, packet.ReferenceCount);
            }

            // Verify that there are no more results (there shouldn't be at this point).
            Assert.Null(channel.ReadInbound<IByteBuffer>());
        }

        [Fact]
        public void MultiplePackets_1ByteLen_ReadsAllBytesFromUnderlyingBuffer()
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

        #endregion

        #region 2 byte length

        [Fact]
        public void OnePacket2ByteLen()
        {
            var channel = new EmbeddedChannel(new GameMessageDecoder());
            var buffer = Unpooled.WrappedBuffer(TwoByteLengthPacket);

            channel.WriteInbound(buffer);

            var result = channel.ReadInbound<IByteBuffer>();

            Assert.NotNull(result);
            var hex = ByteBufferUtil.HexDump(result);
            //Assert.Equal(32, result.GetOpcode());

            // Returns null, as there's no second object returned in the List<object>.
            Assert.Null(channel.ReadInbound<IByteBuffer>());
        }

        [Fact]
        public void MultiplePackets_2ByteLen_ProcessesMultipleInSingleInvocation()
        {

        }

        [Fact]
        public void MultiplePackets_2ByteLen_ReadsAllBytesFromUnderlyingBuffer()
        {

        }

        #endregion

        #region Generic

        [Fact]
        public void DotNettyDoesNotDecreaseReferenceCount()
        {
            var decoder = new GameMessageDecoder();
            var channel = new EmbeddedChannel(decoder);
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
        public void GameMessageDecoderIsNotSharable()
        {
            var decoder = new GameMessageDecoder();

            // DotNetty does not allow sharing of these pipeline items.

            Assert.False(decoder.IsSharable);
        }

        #endregion
    }
}
