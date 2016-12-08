using System.Collections.Generic;
using System.Threading;

namespace OpenClassic.Server.Concurrency
{
    public interface IThreadGuardProvider
    {
        IThreadGuard NewAllowedThreadGuard(Thread allowedThread);

        IThreadGuard NewAllowedThreadGuard(IEnumerable<Thread> allowedThreads);
    }
}
