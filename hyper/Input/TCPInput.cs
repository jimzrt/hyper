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

namespace hyper.Inputs
{

    struct TCPEndpoint
    {
        public TcpClient tcpClient;
        public StreamWriter streamWriter;
    }

    [Target("TCPInput")]
    public sealed class TCPInput : TargetWithLayout, IInput
    {

        private readonly object serverLock = new object();

        List<TCPEndpoint> tcpClients = new List<TCPEndpoint>();

        public bool CanRead { get; set; } = false;


        string currentMessage = "";

        public event ConsoleCancelEventHandler CancelKeyPress;



        private IPAddress ipAddress;
        private int port;


        public TCPInput(int port)
        {
            Name = "TCPInput";
            this.port = port;
            string hostName = Dns.GetHostName();
            Console.WriteLine("hostname: " + hostName);
            IPHostEntry ipHostInfo = Dns.GetHostEntry(hostName);
            this.ipAddress = IPAddress.Any;

            Run();
        }

        public async void Run()
        {
            TcpListener listener = new TcpListener(this.ipAddress, this.port);
            listener.Start();
            Console.Write("Array Min and Avg service is now running");
          
            Console.WriteLine(" on port " + this.port);
            Console.WriteLine("Hit <enter> to stop service\n");
            while (true)
            {
                try
                {
                    TcpClient tcpClient = await listener.AcceptTcpClientAsync();
                    Task t = Process(tcpClient);
                    await t;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }


        private async Task Process(TcpClient tcpClient)
        {
            string clientEndPoint =
              tcpClient.Client.RemoteEndPoint.ToString();
            Console.WriteLine("Received connection request from "
              + clientEndPoint);
            try
            {
                NetworkStream networkStream = tcpClient.GetStream();
                StreamReader reader = new StreamReader(networkStream);
                StreamWriter writer = new StreamWriter(networkStream);
                writer.AutoFlush = true;


                var tcpEndpoint = new TCPEndpoint
                {
                    tcpClient = tcpClient,
                    streamWriter = writer
                };

                tcpClients.Add(tcpEndpoint);
                Console.WriteLine("Clients connected: {0}", tcpClients.Count);

               // writer.AutoFlush = true;
                while (true)
                {
                    string request = await reader.ReadLineAsync();
                    if (request != null)
                    {
                        Console.WriteLine("Received service request: " + request);
                        if (CanRead)
                        {
                            currentMessage = request;
                        }
                        else
                        {
                            if (request == "stop")
                            {
                                CancelKeyPress?.Invoke(null, null);
                            }
                        }

                    }
                    else
                        break; // Client closed connection
                }
                tcpClient.Close();
                lock (serverLock)
                {
                    tcpClients.Remove(tcpEndpoint);
                    Console.WriteLine("Clients connected: {0}", tcpClients.Count);

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                if (tcpClient.Connected)
                    tcpClient.Close();
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


            lock (serverLock)
            {
                foreach (var server in tcpClients)
                {

                    server.streamWriter.WriteLineAsync(logMessage);

                }
            }

        }

    }
}
