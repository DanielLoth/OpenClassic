using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenClassic.Server.Networking
{
    public interface IProtocolProvider
    {
        IPacketHandler[] PacketHandlerMap { get; }
    }
}
