using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace OpenClassic.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var s = new OpenClassic.Server.Networking.Server(new IPEndPoint(IPAddress.Loopback, 43594));
            s.Start();

            var xxx = args;

            Console.WriteLine("Listening...");
            Console.ReadLine();
        }
    }
}
