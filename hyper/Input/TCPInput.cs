using hyper.Input;
using NLog;
using NLog.Targets;
using System;
using System.Net;
using System.Threading;

namespace hyper.Inputs
{
    [Target("TCPInput")]
    public sealed class TCPInput : TargetWithLayout, IInput
    {
        public bool CanRead { get; set; } = false;

        private string currentMessage = "";

        public event ConsoleCancelEventHandler CancelKeyPress;

        private TCPServer server;
        private ManualResetEvent resetEvent;

        public TCPInput(int port)
        {
            Name = "TCPInput";
            server = new TCPServer(IPAddress.Any, port);
            server.OnMessage += OnMessage;
            server.Start();
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
            resetEvent.Set();
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

        public void SetResetEvent(ManualResetEvent resetEvent)
        {
            this.resetEvent = resetEvent;
        }
    }
}