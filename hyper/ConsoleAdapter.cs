using System;
using System.Collections.Generic;
using System.Text;

namespace hyper
{
    class ConsoleAdapter : IAdapter
    {
     
            private int threshold;
            private int total;
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
            CommandHandler?.Invoke(this,command);
              }

        
    }
}
