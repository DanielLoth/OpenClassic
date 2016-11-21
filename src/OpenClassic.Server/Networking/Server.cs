using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OpenClassic.Server.Networking
{
    public class Server
    {
        private readonly IPEndPoint listenerEndpoint;
        private readonly Socket listener;

        private readonly TaskScheduler scheduler;
        private readonly Thread loopThread;

        private readonly EventHandler<SocketAsyncEventArgs> IsCompletedCallback;

        public Server(IPEndPoint endpoint)
        {
            listenerEndpoint = endpoint;
            listener = new Socket(SocketType.Stream, ProtocolType.Tcp);

            scheduler = TaskScheduler.Default; // TODO: Replace
            loopThread = new Thread(Loop)
            {
                IsBackground = true,
                Name = "LoopThread"
            };

            IsCompletedCallback = OnIoCompleted;
        }

        public void Start()
        {
            loopThread.Start();

            listener.Bind(listenerEndpoint);
            listener.Listen(100);
            StartAccept(NewEventArgs());
        }

        private void Loop()
        {
            //Task.Factory.StartNew(async () =>
            //{
            //    while (true)
            //    {
            //        Console.WriteLine($"Looping - On loop thread = {IsOnLoopThread} ({Thread.CurrentThread.Name} - {loopThread.Name})");
            //        Thread.Sleep(1000);
            //        await Task.Delay(100);
            //    }
            //}, CancellationToken.None,
            //TaskCreationOptions.None,
            //scheduler);

            while (true)
            {
                Console.WriteLine($"Looping - On loop thread = {IsOnLoopThread} ({Thread.CurrentThread.Name} - {loopThread.Name})");
                Thread.Sleep(1000);
                //await Task.Delay(100);
            }
        }

        public bool IsOnLoopThread => Thread.CurrentThread == loopThread;

        private SocketOperation NewEventArgs()
        {
            var args = new SocketOperation();

            args.AcceptSocket = null;
            args.Completed += IsCompletedCallback;

            return args;
        }

        private void StartAccept(SocketOperation args)
        {
            if (args == null)
                throw new ArgumentNullException(nameof(args));

            args.AcceptSocket = null;

            var willRaiseEvent = listener.AcceptAsync(args);
            if (!willRaiseEvent)
            {
                ProcessAccept(args);
            }
        }

        private void ProcessAccept(SocketOperation args)
        {
            if (args == null)
                throw new ArgumentNullException(nameof(args));

            var client = new Client();
            var readArgs = new SocketAsyncEventArgs();

            StartAccept(args);
        }

        private void ProcessReceive(SocketOperation args)
        {
            if (args == null)
                throw new ArgumentNullException(nameof(args));
        }

        public void OnIoCompleted(object sender, SocketAsyncEventArgs args)
        {
            if (sender == null)
                throw new ArgumentNullException(nameof(sender));
            if (args == null)
                throw new ArgumentNullException(nameof(args));

            var socketOp = args as SocketOperation;
            if (socketOp == null)
                throw new ArgumentException("Expected to receive an instance of SocketOperation.");

            switch (socketOp.LastOperation)
            {
                case SocketAsyncOperation.Accept:
                    ProcessAccept(socketOp);
                    break;
                case SocketAsyncOperation.Connect:
                    break;
                case SocketAsyncOperation.Receive:
                    break;
                case SocketAsyncOperation.Send:
                    break;
                default:
                    throw new ArgumentException("Invalid socket operation (something other than Accept/Connect/Receive/Send).");
            }
        }
    }
}
