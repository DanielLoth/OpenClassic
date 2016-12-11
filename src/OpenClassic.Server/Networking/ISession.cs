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

        IByteBuffer Buffer { get; }

        int CurrentPacketStartIndex { get; set; }

        int? CurrentPacketId { get; set; }

        int CurrentPacketBitfieldPosition { get; set; }

        int MaxPacketLength { get; }

        Task WriteAndFlushSessionBuffer();

        Task WriteFlushClose();
    }
}
