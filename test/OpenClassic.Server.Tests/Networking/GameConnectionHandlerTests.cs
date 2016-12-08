using DotNetty.Buffers;
using DotNetty.Transport.Channels.Embedded;
using OpenClassic.Server.Networking;
using System.Collections.Generic;
using Xunit;

namespace OpenClassic.Server.Tests.Networking
{
    public class GameConnectionHandlerTests
    {
        readonly byte[] RequestSession = { 32, 55 };
        readonly byte[] SendPrivacySetings = { 64, 1, 2, 3, 4 };

        private GameConnectionHandler NewGameConHandler() => new GameConnectionHandler(new EmbeddedChannel());
        private IByteBuffer NewMessage() => Unpooled.CopiedBuffer(SendPrivacySetings);

        [Fact]
        public void GameConnectionHandlerIsNotSharable()
        {
            var conHandler = NewGameConHandler();

            // DotNetty does not allow sharing of these pipeline items.

            Assert.False(conHandler.IsSharable);
        }

        [Fact]
        public void DotNettyDoesNotDecreaseReferenceCount()
        {
            var conHandler = NewGameConHandler();
            var channel = new EmbeddedChannel(conHandler);
            var packet = Unpooled.WrappedBuffer(RequestSession);

            var startingRefCount = packet.ReferenceCount;

            channel.WriteInbound(packet);

            Assert.Equal(startingRefCount, packet.ReferenceCount);
        }

        [Fact]
        public void AddsNewMessageToPacketQueue()
        {
            var conHandler = NewGameConHandler();
            var channel = new EmbeddedChannel(conHandler);
            var message = NewMessage();

            channel.WriteInbound(message);

            var packetsHandled = conHandler.Pulse();

            Assert.Equal(1, packetsHandled);
        }

        [Fact]
        public void InvokingPulseResultsInPacketProcessingAndQueueClearing()
        {
            var conHandler = NewGameConHandler();
            var channel = new EmbeddedChannel(conHandler);

            for (var i = 0; i < 50; i++)
            {
                channel.WriteInbound(NewMessage());

                var packetsHandled = conHandler.Pulse();

                Assert.Equal(1, packetsHandled);
            }
        }

        [Fact]
        public void InvokingPulseResultsInByteBufferRelease()
        {
            var conHandler = NewGameConHandler();
            var channel = new EmbeddedChannel(conHandler);
            var messages = new List<IByteBuffer>();

            const int messageCount = 50;

            for (var i = 0; i < messageCount; i++)
            {
                var message = NewMessage();

                Assert.Equal(1, message.ReferenceCount);

                messages.Add(message);
            }

            foreach (var message in messages)
            {
                channel.WriteInbound(message);
            }

            conHandler.Pulse();

            foreach (var message in messages)
            {
                Assert.Equal(0, message.ReferenceCount);
            }
        }

        [Fact]
        public void ChannelReadAddsMessageToQueue()
        {
            var conHandler = NewGameConHandler();
            var channel = new EmbeddedChannel(conHandler);
            var chanHandlerCtx = channel.Pipeline.FirstContext();

            const int messageCount = 50;

            for (var i = 0; i < messageCount; i++)
            {
                conHandler.ChannelRead(chanHandlerCtx, NewMessage());
            }

            var packetsHandled = conHandler.Pulse();

            Assert.Equal(messageCount, packetsHandled);
        }
    }
}
