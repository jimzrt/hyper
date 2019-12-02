using System;
using System.Collections.Generic;
using System.Text;
using NLog;
using NLog.Config;
using NLog.Targets;
using Clifton.Core.Pipes;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Net;
using System.Threading.Tasks;
using System.IO;
using NetCoreServer;
using hyper.Input;

namespace hyper.Inputs
{


    [Target("TCPInput")]
    public sealed class TCPInput : TargetWithLayout, IInput
    {


        public bool CanRead { get; set; } = false;

        string currentMessage = "";

        public event ConsoleCancelEventHandler CancelKeyPress;

     //   public List<TcpSession> clients = new List<TcpSession>();



        private TCPServer server;

        public TCPInput(int port)
        {
            Name = "TCPInput";
            server = new TCPServer(IPAddress.Any, port);
            server.OnMessage += OnMessage;
            //    server.OnConnectedE += OnConnected;
           // server.OptionNoDelay = true;

            server.Start();
            //this.port = port;
            //string hostName = Dns.GetHostName();
            //Console.WriteLine("hostname: " + hostName);
            //IPHostEntry ipHostInfo = Dns.GetHostEntry(hostName);
            //this.ipAddress = IPAddress.Any;

            

        }

        //private void OnConnected(TcpSession client)
        //{
        //    Console.WriteLine("added client!");
        //    clients.Add(client);
        //}

        public void OnMessage(string message)
        {
            //server.OnMessage?.Invoke(message);
            if (CanRead)
            {
                currentMessage = message;
            }
            else
            {
                if (message == "stop")
                {
                    CancelKeyPress?.Invoke(null, null);
                }
            }
        }



        public bool Available()
        {
            return currentMessage.Length > 0;
        }

        public string Read()
        {
            var message = currentMessage;
            Flush();
            return message;

        }

        public void Flush()
        {
            currentMessage = "";
        }



        protected override void Write(LogEventInfo logEvent)
        {
            string logMessage = this.Layout.Render(logEvent);



            server.Multicast(logMessage + Environment.NewLine);

            //Console.WriteLine("wrrite to clieont: {0}", logMessage);

                //foreach (var client in clients)
                //{
                //client.Send(logMessage);
                ////    server.streamWriter.WriteLineAsync(logMessage);

                //}
            

        }

    }
}
