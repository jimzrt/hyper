using NetCoreServer;
using System;
using System.Net;
using System.Net.Sockets;

namespace hyper.Input
{
    internal class TCPServer : TcpServer
    {
        public delegate void OnMessageEvent(string message);

        public OnMessageEvent OnMessage;

        public TCPServer(IPAddress address, int port) : base(address, port)
        {
        }

        protected override TcpSession CreateSession()
        {
            return new TCPSession(this);
        }

        protected override void OnError(SocketError error)
        {
            Console.WriteLine($"Chat TCP server caught an error with code {error}");
        }
    }
}