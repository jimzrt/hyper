using Clifton.Core.Pipes;
using System;
using System.IO;
using System.IO.Pipes;

namespace Client
{
    class Program
    {

        public static ClientPipe currentClient = null;

        static void Main(string[] args)
        {

            currentClient = CreateClient();


            currentClient.Connect();
            Console.WriteLine("connected!");




            while (true)
            {
              //  clientPipe.WriteString("hellooo from client!");
               var message =  Console.ReadLine();
                if(message.Length > 0)
                {
                    currentClient?.WriteString(message);

                }
            }
        }


        public static ClientPipe CreateClient()
        {
            ClientPipe clientPipe = new ClientPipe("localhost", "Test", p => p.StartStringReaderAsync());

            clientPipe.DataReceived += (sndr, args) =>
            {
                Console.WriteLine("server: " + args.String);
            };

            clientPipe.PipeClosed += (sdr, args) =>
            {
                Console.WriteLine("pipe closed!");
                currentClient = null;
                var client = CreateClient();
                client.Connect();
                currentClient = client;
                Console.WriteLine("connected!");
            };

            return clientPipe;
        }
    }
}
