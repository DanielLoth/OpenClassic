using DotNetty.Transport.Channels;
using System.Diagnostics;

namespace OpenClassic.Server.Networking
{
    public class GameConnectionHandler : ChannelHandlerAdapter
    {
        public override bool IsSharable => true;

        public override void ChannelActive(IChannelHandlerContext context)
        {
            Debug.Assert(context != null);

            base.ChannelActive(context);
        }

        public override void ChannelRead(IChannelHandlerContext context, object message)
        {
            Debug.Assert(context != null);
            Debug.Assert(message != null);

            base.ChannelRead(context, message);
        }
    }
}
