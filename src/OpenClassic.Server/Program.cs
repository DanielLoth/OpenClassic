using DryIoc;
using OpenClassic.Server.Configuration;
using OpenClassic.Server.Networking;
using System;

namespace OpenClassic.Server
{
    public class Program
    {
        private static readonly GameServer Server;
        private static readonly IGameEngine Engine;

        static Program()
        {
            var resolver = DependencyResolver.Current;

            Server = resolver.Resolve<GameServer>();
            Engine = resolver.Resolve<IGameEngine>();

            // Invoke static constructors here to ensure that they run on the game thread.
            GameConnectionHandler.Init();
        }

#pragma warning disable RECS0154 // Parameter is never used
        public static void Main(string[] args)
#pragma warning restore RECS0154 // Parameter is never used
        {
            Console.WriteLine("Starting listener...");
            Server.Start().Wait();
            Console.WriteLine("Listening");

            Console.CancelKeyPress += Console_CancelKeyPress;

            GC.Collect();

            // This call blocks the main thread until the server is shut down.
            Engine.GameLoop();

            Console.WriteLine("Game loop shut down.");
            Console.WriteLine("Shutdown complete.");
        }

        private static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            Console.WriteLine("Shutting down socket listener...");
            Server.Stop().Wait();

            Console.WriteLine("Shutting down game loop...");
            Engine.StopGameLoop();
        }
    }
}
