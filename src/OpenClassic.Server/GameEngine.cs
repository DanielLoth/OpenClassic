using OpenClassic.Server.Networking;
using System;
using System.Collections.Generic;
using System.Threading;

namespace OpenClassic.Server
{
    public class GameEngine : IGameEngine
    {
        private static readonly Thread GameThread;

        private volatile bool IsRunning = true;

        private readonly List<Action> TaskQueue = new List<Action>(2000);
        private readonly object TaskQueueLock = new object();

        private readonly List<ISession> Sessions = new List<ISession>();

        static GameEngine()
        {
            GameThread = Thread.CurrentThread;
        }

        public void GameLoop()
        {
            var currentThread = Thread.CurrentThread;

            if (currentThread != GameThread)
            {
                throw new InvalidOperationException("A thread that is not the game thread attempted to invoke GameEngine.GameLoop().");
            }

            if (!IsRunning)
            {
                throw new InvalidOperationException("An attempt has been made to re-enter GameEngine.GameLoop() even though the game loop has been shutdown.");
            }

            long counter = 0;
            while (IsRunning)
            {
                //Console.WriteLine(counter++ % 2 == 0 ? "Tick" : "Tock");

                PulseSessions();
                ProcessTasks();

                Thread.Sleep(600);
            }
        }

        public void StopGameLoop()
        {
            IsRunning = false;
        }

        public void RegisterSession(ISession session)
        {
            Sessions.Add(session);
        }

        public void UnregisterSession(ISession session)
        {
            Sessions.Remove(session);
        }

        public void QueueGameLoopTask(Action action)
        {
            lock (TaskQueueLock)
            {
                TaskQueue.Add(action);
            }
        }

        private void PulseSessions()
        {
            foreach (var session in Sessions)
            {
                session.Pulse();
            }
        }

        private void ProcessTasks()
        {
            lock(TaskQueueLock)
            {
                foreach (var action in TaskQueue)
                {
                    action.Invoke();
                }
            }
        }
    }
}
