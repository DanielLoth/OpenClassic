using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using System.Threading.Tasks;

namespace OpenClassic.Server.Networking
{
    public interface ISession
    {
        IChannel ClientChannel { get; }

        int Pulse();

        bool AllowedToDisconnect { get; }

        Task WriteAndFlushAsync(IByteBuffer buffer);
    }
}
