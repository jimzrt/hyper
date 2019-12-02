using NetCoreServer;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace hyper.Input
{
    class TCPServer : TcpServer
    {

        public delegate void OnMessageEvent(string message);
        public OnMessageEvent OnMessage;

        public TCPServer(IPAddress address, int port) : base(address, port) { }

        protected override TcpSession CreateSession() { return new TCPSession(this); }


        protected override void OnError(SocketError error)
        {
            Console.WriteLine($"Chat TCP server caught an error with code {error}");
        }
    }
}
