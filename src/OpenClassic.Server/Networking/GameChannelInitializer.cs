using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using System.Diagnostics;

namespace OpenClassic.Server.Networking
{
    public class GameChannelInitializer : ChannelInitializer<ISocketChannel>
    {
        static readonly GameConnectionHandler SharedGameConnectionHandler = new GameConnectionHandler();
        static readonly GameMessageDecoder SharedGameMessageDecoder = new GameMessageDecoder();

        protected override void InitChannel(ISocketChannel channel)
        {
            Debug.Assert(channel != null);

            var pipeline = channel.Pipeline;

            pipeline.AddLast(SharedGameMessageDecoder);
            pipeline.AddLast(SharedGameConnectionHandler);
        }
    }
}
