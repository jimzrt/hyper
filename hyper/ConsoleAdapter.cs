using System;

namespace hyper
{
    internal class ConsoleAdapter : IAdapter
    {

        private readonly int threshold;
        private readonly int total;
        private bool running = false;
        public ConsoleAdapter()
        {
            Console.WriteLine("Adapter Created");
        }

        public event EventHandler<string> CommandHandler;

        public void Listen()
        {
            Console.WriteLine("consoleadapter listening!");
            running = true;
            while (running)
            {
                var input = Console.ReadLine();
                OnCommand(input);

            }


        }

        public void WriteLine(string line)
        {
            Console.WriteLine(line);
        }



        protected virtual void OnCommand(string command)
        {
            CommandHandler?.Invoke(this, command);
        }


    }
}
