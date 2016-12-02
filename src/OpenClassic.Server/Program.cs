using System;
using System.Threading;
using System.Threading.Tasks;

namespace OpenClassic.Server
{
    public class Program
    {
        private static readonly Networking.Server Server = new Networking.Server();

        public static void Main(string[] args)
        {
            Console.WriteLine("Starting listener...");
            Server.Start().Wait();
            Console.WriteLine("Listening");

            Console.CancelKeyPress += Console_CancelKeyPress;

            for (;;) { Thread.Sleep(1000); }
        }

        private static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            Console.WriteLine("Shutting down...");
            Server.Stop().Wait();
            Console.WriteLine("Shutdown complete. Press any key to terminate this window.");

            Console.ReadLine();
        }
    }
}
