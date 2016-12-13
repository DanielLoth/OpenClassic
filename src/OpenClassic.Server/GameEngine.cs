using OpenClassic.Server.Networking;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace OpenClassic.Server
{
    public class GameEngine : IGameEngine
    {
        private static readonly Thread GameThread;

        private volatile bool IsRunning = true;

        private readonly List<Action> TaskQueue = new List<Action>(2000);

        private readonly List<ISession> Sessions = new List<ISession>();

        private readonly ISessionUpdater SessionUpdater;

        public bool IsOnGameThread => Thread.CurrentThread == GameThread;

        static GameEngine()
        {
            GameThread = Thread.CurrentThread;
        }

        public GameEngine(ISessionUpdater updater)
        {
            Debug.Assert(updater != null);

            SessionUpdater = updater;
        }

        public void GameLoop()
        {
            if (!IsOnGameThread)
            {
                throw new InvalidOperationException("A thread that is not the game thread attempted to invoke GameEngine.GameLoop().");
            }

            if (!IsRunning)
            {
                throw new InvalidOperationException("An attempt has been made to re-enter GameEngine.GameLoop() even though the game loop has been shutdown.");
            }

            while (IsRunning)
            {
                // RSCD uses the following approach:
                // 1. processLoginServer()
                // 2. processIncomingPackets()
                // 3. processEvents()
                // 4. processClients()

                PulseSessions(); // Processes inbound packets.
                ProcessTasks(); // Runs any queued scheduled to run this tick.
                SendClientUpdates();

                Thread.Sleep(50);
            }

            // TODO: Loop stopped - gracefully disconnect all users.
        }

        public void StopGameLoop()
        {
            if (IsOnGameThread)
            {
                IsRunning = false;
            }
            else
            {
                QueueGameLoopTask(StopGameLoop);
            }
        }

        public void RegisterSession(ISession session)
        {
            Debug.Assert(session != null);

            if (IsOnGameThread)
            {
                Sessions.Add(session);
            }
            else
            {
                QueueGameLoopTask(() => RegisterSession(session));
            }
        }

        public void UnregisterSession(ISession session)
        {
            Debug.Assert(session != null);

            if (IsOnGameThread)
            {
                Sessions.Remove(session);
            }
            else
            {
                QueueGameLoopTask(() => UnregisterSession(session));
            }
        }

        public void QueueGameLoopTask(Action action)
        {
            Debug.Assert(action != null);

            var taskQueue = TaskQueue;
            Debug.Assert(taskQueue != null);

            lock (taskQueue)
            {
                taskQueue.Add(action);
            }
        }

        private void PulseSessions()
        {
            Debug.Assert(Sessions != null);

            foreach (var session in Sessions)
            {
                session.Pulse();
            }
        }

        private void ProcessTasks()
        {
            var taskQueue = TaskQueue;

            Debug.Assert(taskQueue != null);

            lock (taskQueue)
            {
                foreach (var action in taskQueue)
                {
                    action.Invoke();
                }

                taskQueue.Clear();
                Debug.Assert(taskQueue.Count == 0);
            }
        }

        private void SendClientUpdates()
        {
            Debug.Assert(Sessions != null);

            // TODO: Update world first (inject IWorldUpater interface into the World class, and use it).
            // 1. Update NPC positions
            // 2. Update player positions
            // 3. Update message queues (i.e., chat messages)
            // 4. Update trade/duel offers.

            foreach (var session in Sessions)
            {
                if (session.ShouldUpdate)
                {
                    SessionUpdater.Update(session);
                }
            }

            // TODO: Update collections (e.g.: watched entities, etc).
            // In the world class? Or here? Unsure.
        }
    }
}
