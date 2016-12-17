using DotNetty.Buffers;
using DotNetty.Transport.Channels.Embedded;
using DryIoc;
using OpenClassic.Server.Configuration;
using OpenClassic.Server.Networking;
using System.Collections.Generic;
using Xunit;

namespace OpenClassic.Server.Tests.Networking
{
    public class GameConnectionHandlerTests
    {
        readonly byte[] RequestSession = { 32, 55 };
        readonly byte[] SendPrivacySetings = { 64, 1, 2, 3, 4 };

        private readonly GameConnectionHandler ConnectionHandler;

        private IByteBuffer NewMessage() => Unpooled.CopiedBuffer(SendPrivacySetings);

        public GameConnectionHandlerTests()
        {
            var container = DependencyResolver.Current;
            var gameEngine = container.Resolve<IGameEngine>();
            var packetHandlers = container.Resolve<IPacketHandler[]>();
            
            GameConnectionHandler.Init(gameEngine, packetHandlers);
            ConnectionHandler = new GameConnectionHandler(new EmbeddedChannel());
        }

        [Fact]
        public void GameConnectionHandlerIsNotSharable()
        {
            // DotNetty does not allow sharing of these pipeline items.

            Assert.False(ConnectionHandler.IsSharable);
        }

        [Fact]
        public void DotNettyDoesNotDecreaseReferenceCount()
        {
            var channel = new EmbeddedChannel(ConnectionHandler);
            var packet = Unpooled.WrappedBuffer(RequestSession);

            var startingRefCount = packet.ReferenceCount;

            channel.WriteInbound(packet);

            Assert.Equal(startingRefCount, packet.ReferenceCount);
        }

        [Fact]
        public void AddsNewMessageToPacketQueue()
        {
            var channel = new EmbeddedChannel(ConnectionHandler);
            var message = NewMessage();

            channel.WriteInbound(message);

            var packetsHandled = ConnectionHandler.Pulse();

            Assert.Equal(1, packetsHandled);
        }

        [Fact]
        public void InvokingPulseResultsInPacketProcessingAndQueueClearing()
        {
            var channel = new EmbeddedChannel(ConnectionHandler);

            for (var i = 0; i < 50; i++)
            {
                channel.WriteInbound(NewMessage());

                var packetsHandled = ConnectionHandler.Pulse();

                Assert.Equal(1, packetsHandled);
            }
        }

        [Fact]
        public void InvokingPulseResultsInByteBufferRelease()
        {
            var channel = new EmbeddedChannel(ConnectionHandler);
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

            ConnectionHandler.Pulse();

            foreach (var message in messages)
            {
                Assert.Equal(0, message.ReferenceCount);
            }
        }

        [Fact]
        public void ChannelReadAddsMessageToQueue()
        {
            var channel = new EmbeddedChannel(ConnectionHandler);
            var chanHandlerCtx = channel.Pipeline.FirstContext();

            const int messageCount = 50;

            for (var i = 0; i < messageCount; i++)
            {
                ConnectionHandler.ChannelRead(chanHandlerCtx, NewMessage());
            }

            var packetsHandled = ConnectionHandler.Pulse();

            Assert.Equal(messageCount, packetsHandled);
        }
    }
}
