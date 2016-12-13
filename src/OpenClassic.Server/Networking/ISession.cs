using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using OpenClassic.Server.Domain;
using System.Threading.Tasks;

namespace OpenClassic.Server.Networking
{
    public interface ISession
    {
        IChannel ClientChannel { get; }

        int Pulse();

        bool AllowedToDisconnect { get; }

        IByteBuffer Buffer { get; }

        IPlayer Player { get; set; }

        bool ShouldUpdate { get; }

        int CurrentPacketStartIndex { get; set; }

        int? CurrentPacketId { get; set; }

        int CurrentPacketBitfieldPosition { get; set; }

        int MaxPacketLength { get; }

        Task WriteAndFlushSessionBuffer();

        Task WriteFlushClose();
    }
}
