using System;
using System.Collections.Generic;
using System.Text;
using NLog;
using NLog.Config;
using NLog.Targets;
using Clifton.Core.Pipes;
using System.Collections.Concurrent;

namespace hyper.Inputs
{
    [Target("PipeInput")]
    public sealed class PipeInput : TargetWithLayout, IInput
    {

        private readonly object serverLock = new object();

        

        //List<ServerPipe> serverPipes = new List<ServerPipe>();
        //ConcurrentQueue<ServerPipe> serverPipes = new ConcurrentQueue<ServerPipe>();
        // ConcurrentDictionary<ServerPipe, ServerPipe> serverPipes = new ConcurrentDictionary<ServerPipe, ServerPipe>();
        List<ServerPipe> serverPipes = new List<ServerPipe>();

        public bool CanRead { get; set; } = false;

     //   private bool CanSend { get; set; } = false;

        string currentMessage = "";

        public event ConsoleCancelEventHandler CancelKeyPress;

        public PipeInput()
        {

            Name = "PipeInput";
            var server = CreateServer();
            serverPipes.Add(server);
            


        }

        public ServerPipe CreateServer()
        {
            var serverPipe = new ServerPipe("Test", p => p.StartStringReaderAsync());


            serverPipe.DataReceived += DataReceived;

            serverPipe.PipeClosed += (sndr, args) =>
            {
                Console.WriteLine("client disconnected!");


                //       CanSend = false;
                serverPipe.isConnected = false;
                lock (serverLock)
                {
                    serverPipes.Remove(serverPipe);

                }
                Console.WriteLine("connected clients: " + serverPipes.Count);
            };


            serverPipe.Connected += (sndr, args) =>
            {
                // Console.WriteLine(sndr.ToString() + " connected!");
                Console.WriteLine("COOONNECCTOS!");
               

                var newServer = CreateServer();
                lock (serverLock)
                {
                    serverPipes.Add(newServer);
                }
                Console.WriteLine("connected clients: " + serverPipes.Count);
                //   CanSend = true;

            };

            serverPipe.Start();
            return serverPipe;
        }


        private void DataReceived(object sender, PipeEventArgs args)
        {

            if (CanRead)
            {
                currentMessage = args.String;
            } else
            {
                if(args.String == "stop")
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

            if (true)
            {
                lock (serverLock)
                {
                    foreach (var server in serverPipes)
                    {
                        if (server.isConnected)
                        {
                            server.WriteString(logMessage);

                        }
                    }
                }


            }


        }

    }
}
