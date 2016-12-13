using DotNetty.Buffers;
using System.Diagnostics;

namespace OpenClassic.Server.Networking.Rscd
{
    internal class DepositBankItemPacketHandler : IPacketHandler
    {
        public int Opcode => 198;

        public void Handle(ISession session, IByteBuffer packet)
        {
            Debug.Assert(session != null);
            Debug.Assert(packet != null);
        }
    }

    internal class WithdrawBankItemPacketHandler : IPacketHandler
    {
        public int Opcode => 183;

        public void Handle(ISession session, IByteBuffer packet)
        {
            Debug.Assert(session != null);
            Debug.Assert(packet != null);
        }
    }
}
