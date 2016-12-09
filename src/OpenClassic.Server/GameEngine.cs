using System;
using System.Threading;

namespace OpenClassic.Server
{
    public class GameEngine : IGameEngine
    {
        private static readonly Thread GameThread;

        private volatile bool IsRunning = true;

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
                Console.WriteLine(counter++ % 2 == 0 ? "Tick" : "Tock");

                Thread.Sleep(600);
            }
        }

        public void StopGameLoop()
        {
            IsRunning = false;
        }
    }
}
