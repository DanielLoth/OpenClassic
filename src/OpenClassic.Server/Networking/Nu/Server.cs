using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace OpenClassic.Server.Networking.Nu
{
    class Server
    {
        private int m_receiveBufferSize;// buffer size to use for each socket I/O operation 
        Socket listenSocket;            // the socket used to listen for incoming connection requests
        int m_totalBytesRead;           // counter of the total # bytes received by the server
        int m_numConnectedSockets;      // the total number of clients connected to the server 

        //private byte[] buffer = new byte[32768000];

        private readonly SocketAsyncEventArgs AcceptArgs = new SocketAsyncEventArgs();

        // Create an uninitialized server instance.  
        // To start the server listening for connection requests
        // call the Init method followed by Start method 
        //
        // <param name="numConnections">the maximum number of connections the sample is designed to handle simultaneously</param>
        // <param name="receiveBufferSize">buffer size to use for each socket I/O operation</param>
        public Server(int receiveBufferSize)
        {
            m_totalBytesRead = 0;
            m_numConnectedSockets = 0;
            m_receiveBufferSize = receiveBufferSize;

            AcceptArgs.Completed += new EventHandler<SocketAsyncEventArgs>(AcceptCompletion);
            AcceptArgs.AcceptSocket = null;
        }

        // Starts the server such that it is listening for 
        // incoming connection requests.    
        //
        // <param name="localEndPoint">The endpoint which the server will listening 
        // for connection requests on</param>
        public void Start(IPEndPoint localEndPoint)
        {
            // create the socket which listens for incoming connections
            listenSocket = new Socket(localEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            listenSocket.Bind(localEndPoint);
            // start the server with a listen backlog of 100 connections
            listenSocket.Listen(100);

            // post accepts on the listening socket
            StartAccept();

            //Console.WriteLine("{0} connected sockets with one outstanding receive posted to each....press any key", m_outstandingReadCount);
            Console.WriteLine("Press any key to terminate the server process....");
            for (;;)
            {
                Thread.Sleep(500);
            }
        }

        private SocketAsyncEventArgs NewArgs()
        {
            var args = new SocketAsyncEventArgs();
            args.Completed += new EventHandler<SocketAsyncEventArgs>(IO_Completed);

            var buffer = new byte[m_receiveBufferSize];
            args.SetBuffer(buffer, 0, buffer.Length);

            var token = new Session();
            args.UserToken = token;

            return args;
        }


        // Begins an operation to accept a connection request from the client 
        //
        // <param name="acceptEventArg">The context object to use when issuing 
        // the accept operation on the server's listening socket</param>
        public void StartAccept()
        {
            var willRaiseEvent = false;
            var acceptArgs = AcceptArgs;

            try
            {
                willRaiseEvent = listenSocket.AcceptAsync(acceptArgs);
            }
            catch (SocketException)
            {
                //break;
            }
            catch (ObjectDisposedException)
            {
                //break;
            }

            if (!willRaiseEvent)
            {
                ProcessAccept(acceptArgs);
            }
        }

        // This method is the callback method associated with Socket.AcceptAsync 
        // operations and is invoked when an accept operation is complete
        //
        void AcceptCompletion(object sender, SocketAsyncEventArgs e)
        {
            ProcessAccept(e);
            StartAccept();
        }

        private void ProcessAccept(SocketAsyncEventArgs e)
        {
            Interlocked.Increment(ref m_numConnectedSockets);
            Console.WriteLine("Client connection accepted. There are {0} clients connected to the server",
                m_numConnectedSockets);

            var token = new Session();
            token.Socket = e.AcceptSocket;

            token.Start();

            e.AcceptSocket = null;
        }

        // This method is called whenever a receive or send operation is completed on a socket 
        //
        // <param name="e">SocketAsyncEventArg associated with the completed receive operation</param>
        void IO_Completed(object sender, SocketAsyncEventArgs e)
        {
            // determine which type of operation just completed and call the associated handler
            switch (e.LastOperation)
            {
                case SocketAsyncOperation.Receive:
                    //ProcessReceive(e);
                    break;
                case SocketAsyncOperation.Send:
                    //ProcessSend(e);
                    break;
                default:
                    throw new ArgumentException("The last operation completed on the socket was not a receive or send");
            }

        }

        private void CloseClientSocket(SocketAsyncEventArgs e)
        {
            var token = (Session)e.UserToken;

            // close the socket associated with the client
            try
            {
                token.Socket.Shutdown(SocketShutdown.Send);
            }
            // throws if client process has already closed
            catch (Exception) { }
            token.Socket.Shutdown(SocketShutdown.Both);

            // decrement the counter keeping track of the total number of clients connected to the server
            Interlocked.Decrement(ref m_numConnectedSockets);
            Console.WriteLine("A client has been disconnected from the server. There are {0} clients connected to the server", m_numConnectedSockets);
        }
    }
}
