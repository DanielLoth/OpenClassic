using OpenClassic.Server.Networking;
using System;

namespace OpenClassic.Server
{
    public interface IGameEngine
    {
        bool IsOnGameThread { get; }

        void GameLoop();
        void StopGameLoop();

        void RegisterSession(ISession session);

        void UnregisterSession(ISession session);

        void QueueGameLoopTask(Action action);
    }
}
