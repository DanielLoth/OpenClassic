using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace OpenClassic.Server.Networking
{
    public class SocketOperation : SocketAsyncEventArgs
    {
        private static int Counter = 1;

        private readonly int Id;

        public SocketOperation()
        {
            Id = Interlocked.Increment(ref Counter);
        }
    }
}
