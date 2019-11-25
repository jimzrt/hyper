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
            Program p = new Program(5432);
            p.Run();
            //while (true)
            //{
            //    Task.Delay(1000).Wait();
            //}

        }

        public Program(int port)
        {
           // Run();
        }

        public void Run()
        {
            TcpClient client = new TcpClient("localhsot", 5432);
            NetworkStream stream = client.GetStream();
            StreamReader reader = new StreamReader(stream);
            StreamWriter writer = new StreamWriter(stream);
            writer.AutoFlush = true;
            Task.Run(() =>
            {
                while (true)
                {
                    try
                    {

             
                    string request = reader.ReadLine();
                    if (request != null)
                    {
                        Console.WriteLine(request);
                    }
                    } catch(Exception e)
                    {
                        Console.WriteLine(e.Message);
                        return;
                    }
                }
                

            });
           
            while (true)
            {
                string message = Console.ReadLine();
                writer.WriteLine(message);
            }


        }




    }
}
