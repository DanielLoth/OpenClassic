using DryIoc;
using OpenClassic.Server.Configuration;
using OpenClassic.Server.Networking;
using System;

namespace OpenClassic.Server
{
    public class Program
    {
        private static readonly GameServer Server = DependencyResolver.Current.Resolve<GameServer>();
        private static readonly IGameEngine Engine = DependencyResolver.Current.Resolve<IGameEngine>();

#pragma warning disable RECS0154 // Parameter is never used
        public static void Main(string[] args)
#pragma warning restore RECS0154 // Parameter is never used
        {
            Console.WriteLine("Starting listener...");
            Server.Start().Wait();
            Console.WriteLine("Listening");

            var configProvider = new JsonConfigProvider();
            var config = configProvider.GetConfig();

            Console.CancelKeyPress += Console_CancelKeyPress;

            Engine.GameLoop();
        }

        private static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            Console.WriteLine("Shutting down socket listener...");
            Server.Stop().Wait();

            Console.WriteLine("Shutting down game loop...");
            Engine.StopGameLoop();

            Console.WriteLine("Shutdown complete. Press any key to terminate this window.");
        }
    }
}
