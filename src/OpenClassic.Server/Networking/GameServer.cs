using DotNetty.Buffers;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using System;
using System.Threading.Tasks;

namespace OpenClassic.Server.Networking
{
    public class GameServer
    {
        private readonly IEventLoopGroup BossGroup;
        private readonly IEventLoopGroup WorkerGroup;

        private IChannel BootstrapChannel;

        public GameServer()
        {
            BossGroup = new MultithreadEventLoopGroup(1);
            WorkerGroup = new MultithreadEventLoopGroup(1);
        }

        public async Task Start()
        {
            if (BootstrapChannel != null)
            {
                throw new InvalidOperationException("This Server object has already been started.");
            }

            var bootstrap = new ServerBootstrap()
                .Group(BossGroup, WorkerGroup)
                .Channel<TcpServerSocketChannel>()
                .Option(ChannelOption.Allocator, PooledByteBufferAllocator.Default)
                .Option(ChannelOption.SoBacklog, 100)
                .Option(ChannelOption.TcpNodelay, true)
                .ChildOption(ChannelOption.Allocator, PooledByteBufferAllocator.Default)
                .ChildOption(ChannelOption.SoKeepalive, true)
                .ChildOption(ChannelOption.TcpNodelay, true)
                .ChildHandler(new GameChannelInitializer());

            BootstrapChannel = await bootstrap.BindAsync(43594);
        }

        public async Task Stop()
        {
            if (BootstrapChannel == null)
            {
                throw new InvalidOperationException("This Server object has already been stopped (or was never started).");
            }

            try
            {
                await BootstrapChannel.CloseAsync();
            }
            finally
            {
                await Task.WhenAll(BossGroup.ShutdownGracefullyAsync(), WorkerGroup.ShutdownGracefullyAsync());
            }
        }
    }
}
