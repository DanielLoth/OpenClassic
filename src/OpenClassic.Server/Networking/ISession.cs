using DotNetty.Transport.Channels;

namespace OpenClassic.Server.Networking
{
    public interface ISession
    {
        IChannel ClientChannel { get; }

        int Pulse();

        bool AllowedToDisconnect { get; }
    }
}
