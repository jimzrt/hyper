using System;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace ClientTCP
{
    class Program
    {

        static void Main(string[] args)
        {
            string address = "127.0.0.1";
            if (args.Length > 0)
                address = args[0];

            var client = new TCPClient(address, 5432);

            // Connect the client
            Console.Write("Client connecting...");
            client.ConnectAsync();
            Console.WriteLine("Done!");


            while (true)
            {
                string line = Console.ReadLine();
                if (!string.IsNullOrEmpty(line))
                {
                    client.SendAsync(line);
                }
            }

        }




    }
}
