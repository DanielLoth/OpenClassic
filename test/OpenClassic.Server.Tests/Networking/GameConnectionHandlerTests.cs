using DotNetty.Buffers;
using DotNetty.Transport.Channels.Embedded;
using OpenClassic.Server.Networking;
using Xunit;

namespace OpenClassic.Server.Tests.Networking
{
    public class GameConnectionHandlerTests
    {
        readonly byte[] RequestSession = { 32, 55 };
        readonly byte[] SendPrivacySetings = { 5, 4, 64, 1, 2, 3 };

        [Fact]
        public void DotNettyDoesNotDecreaseReferenceCount()
        {
            var channel = new EmbeddedChannel(new GameConnectionHandler(new EmbeddedChannel()));
            var packet = Unpooled.WrappedBuffer(RequestSession);

            var startingRefCount = packet.ReferenceCount;

            channel.WriteInbound(packet);

            Assert.Equal(startingRefCount, packet.ReferenceCount);
        }
    }
}
