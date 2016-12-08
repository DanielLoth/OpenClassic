using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace OpenClassic.Server.Concurrency
{
    public class NoOpThreadGuard : IThreadGuard
    {
        public NoOpThreadGuard(Thread allowedThread)
        {
            Debug.Assert(allowedThread != null);

            // Do nothing - this is the no-operation implementation.
        }

        public NoOpThreadGuard(IEnumerable<Thread> allowedThreads)
        {
            Debug.Assert(allowedThreads != null);

            // Do nothing - this is the no-operation implementation.
        }

        public void CheckCurrentThreadAllowed()
        {
            // Do nothing - this is the no-operation implementation.
        }
    }
}
