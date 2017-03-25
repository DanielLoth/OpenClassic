using DotNetty.Buffers;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace OpenClassic.Server.Networking
{
    public class NettyGameServer : IGameServer
    {
        private readonly IEventLoopGroup BossGroup;
        private readonly IEventLoopGroup WorkerGroup;
        private readonly ChannelInitializer<ISocketChannel> ChannelInitializer;

        private IChannel BootstrapChannel;

        public NettyGameServer(ChannelInitializer<ISocketChannel> channelInitializer)
        {
            Debug.Assert(channelInitializer != null);

            BossGroup = new MultithreadEventLoopGroup(1);
            WorkerGroup = new MultithreadEventLoopGroup(1);

            ChannelInitializer = channelInitializer;
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
                .Option(ChannelOption.Allocator, UnpooledByteBufferAllocator.Default)
                .Option(ChannelOption.SoBacklog, 10)
                .Option(ChannelOption.TcpNodelay, true)
                .ChildOption(ChannelOption.Allocator, UnpooledByteBufferAllocator.Default)
                .ChildOption(ChannelOption.TcpNodelay, true)
                .ChildHandler(ChannelInitializer);

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
