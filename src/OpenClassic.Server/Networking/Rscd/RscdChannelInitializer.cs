using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using System.Diagnostics;

namespace OpenClassic.Server.Networking.Rscd
{
    public class RscdChannelInitializer : ChannelInitializer<ISocketChannel>
    {
        private readonly IGameEngine engine;

        public RscdChannelInitializer(IGameEngine engine)
        {
            Debug.Assert(engine != null);

            this.engine = engine;
        }

        protected override void InitChannel(ISocketChannel channel)
        {
            Debug.Assert(channel != null);

            var pipeline = channel.Pipeline;

            pipeline.AddLast(new GameMessageDecoder());

            var session = new GameConnectionHandler(channel);
            pipeline.AddLast(session);

            engine.RegisterSession(session);
        }
    }
}
