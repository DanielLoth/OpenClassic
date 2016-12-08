using OpenClassic.Server.Concurrency;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Xunit;

namespace OpenClassic.Server.Tests.Concurrency
{
    public class ThrowExceptionThreadGuardTests
    {
        [Fact]
        public void DoesNotThrowWhenCalledFromAllowedThread_SingleAllowedThread()
        {
            var currentThread = Thread.CurrentThread;
            var guard = new ThrowExceptionThreadGuard(currentThread);

            guard.CheckCurrentThreadAllowed();

            // Reaching this point means that the test passes, as no exception
            // was thrown.
        }

        [Fact]
        public void DoesNotThrowWhenCalledFromAllowedThread_MultipleAllowedThreads()
        {
            const int allowedThreadCount = 5;
            var lockObject = new object();
            ThrowExceptionThreadGuard guard = null;

            var allowedThreadExceptionCount = 0;

            var allowedThreads = new List<Thread>();
            for (var i = 0; i < allowedThreadCount; i++)
            {
                var allowedThread = new Thread(() =>
                {
                    lock (lockObject)
                    {
                        Debug.Assert(guard != null);

                        try
                        {
                            guard.CheckCurrentThreadAllowed();
                        }
                        catch
                        {
                            allowedThreadExceptionCount++;
                        }
                    }
                });

                allowedThreads.Add(allowedThread);
            }

            lock (lockObject)
            {
                guard = new ThrowExceptionThreadGuard(allowedThreads);
            }

            foreach (var thread in allowedThreads)
            {
                thread.Start();
                thread.Join();
            }

            Assert.Equal(0, allowedThreadExceptionCount);

            // Reaching this point means that the test passes, as no exception
            // was thrown.
        }

        [Fact]
        public void ThrowsWhenCalledFromDisallowedThread_SingleAllowedThread()
        {
            var allowedThread = new Thread(() => { });
            var guard = new ThrowExceptionThreadGuard(allowedThread);

            Assert.Throws<WrongThreadException>(() => guard.CheckCurrentThreadAllowed());
        }

        [Fact]
        public void ThrowsWhenCalledFromDisallowedThread_MultipleAllowedThreads()
        {
            const int threadCount = 5;
            var lockObject = new object();
            ThrowExceptionThreadGuard guard = null;

            var allowedThreadExceptionCount = 0;
            var disallowedThreadExceptionCount = 0;

            var allowedThreads = new List<Thread>();
            for (var i = 0; i < threadCount; i++)
            {
                var allowedThread = new Thread(() =>
                {
                    lock (lockObject)
                    {
                        Debug.Assert(guard != null);

                        try
                        {
                            guard.CheckCurrentThreadAllowed();
                        }
                        catch
                        {
                            allowedThreadExceptionCount++;
                        }
                    }
                });

                allowedThreads.Add(allowedThread);
            }

            var disallowedThreads = new List<Thread>();
            for (var i = 0; i < threadCount; i++)
            {
                var disallowedThread = new Thread(() =>
                {
                    lock (lockObject)
                    {
                        Debug.Assert(guard != null);

                        try
                        {
                            guard.CheckCurrentThreadAllowed();
                        }
                        catch
                        {
                            disallowedThreadExceptionCount++;
                        }
                    }
                });

                disallowedThreads.Add(disallowedThread);
            }

            lock (lockObject)
            {
                guard = new ThrowExceptionThreadGuard(allowedThreads);
            }

            foreach (var thread in allowedThreads)
            {
                // Each thread started here will call guard.CheckCurrentThreadAllowed()
                // and no exceptions will be thrown.
                thread.Start();
                thread.Join();
            }

            foreach (var thread in disallowedThreads)
            {
                // Each thread started here will throw an exception when the
                // CheckCurrentThreadAllowed() method is called.
                thread.Start();
                thread.Join();
            }

            Assert.Equal(0, allowedThreadExceptionCount);
            Assert.Equal(threadCount, disallowedThreadExceptionCount);
        }
    }
}
