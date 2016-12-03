using DotNetty.Transport.Channels;

namespace OpenClassic.Server.Networking
{
    public interface ISession
    {
        IChannel ClientChannel { get; }

        bool Pulse();
        bool AllowedToDisconnect { get; }
    }
}
