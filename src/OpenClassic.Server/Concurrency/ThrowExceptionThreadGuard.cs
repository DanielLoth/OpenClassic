using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace OpenClassic.Server.Concurrency
{
    public class ThrowExceptionThreadGuard : IThreadGuard
    {
        private readonly Thread singleAllowedThread;
        private readonly HashSet<Thread> allowedThreads;

        private readonly Action checkAction;

        public ThrowExceptionThreadGuard(Thread allowedThread)
        {
            Debug.Assert(allowedThread != null);

            singleAllowedThread = allowedThread;
            checkAction = CheckCurrentThreadAllowedSingleThreadCheck;
        }

        public ThrowExceptionThreadGuard(IEnumerable<Thread> allowed)
        {
            Debug.Assert(allowed != null);

            var allowedSet = new HashSet<Thread>();
            foreach (var thread in allowed)
            {
                allowedSet.Add(thread);
            }

            allowedThreads = allowedSet;
            checkAction = CheckCurrentThreadAllowedMultipleThreadCheck;
        }

        public void CheckCurrentThreadAllowed()
        {
            Debug.Assert(checkAction != null);

            checkAction();
        }

        private void CheckCurrentThreadAllowedSingleThreadCheck()
        {
            Debug.Assert(singleAllowedThread != null);

            var currentThread = Thread.CurrentThread;

            if (currentThread != singleAllowedThread)
            {
                var errorMessage = $"The method you've called does not allow invocation from the current thread. " +
                    $"Allowed thread ID: {singleAllowedThread.ManagedThreadId} - " +
                    $"Calling (disallowed) thread ID: {currentThread.ManagedThreadId}";

                throw new WrongThreadException(errorMessage);
            }
        }

        private void CheckCurrentThreadAllowedMultipleThreadCheck()
        {
            Debug.Assert(allowedThreads != null);

            var currentThread = Thread.CurrentThread;

            foreach (var allowedThread in allowedThreads)
            {
                if (currentThread == allowedThread)
                {
                    return; // The calling thread is allowed to invoke the current code.
                }
            }

            // If we reach this point then the calling (current) thread is not
            // supposed to be accessing the method it just called. Throw exception.

            throw new WrongThreadException("The method you've called does not allow invocation from the current thread.");
        }
    }
}
