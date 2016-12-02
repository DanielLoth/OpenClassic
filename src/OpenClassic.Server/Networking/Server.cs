using DotNetty.Codecs;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace OpenClassic.Server.Networking
{
    public class Server
    {
        private readonly IEventLoopGroup BossGroup;
        private readonly IEventLoopGroup WorkerGroup;

        private IChannel BootstrapChannel;

        public Server()
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
                .Option(ChannelOption.SoBacklog, 100)
                .ChildHandler(new ActionChannelInitializer<ISocketChannel>(channel =>
                {
                    var pipeline = channel.Pipeline;

                    pipeline.AddLast(new DelimiterBasedFrameDecoder(8192, Delimiters.LineDelimiter()));
                }));

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
