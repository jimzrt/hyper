using System;
using System.Collections.Generic;
using System.Text;
using NLog;
using NLog.Config;
using NLog.Targets;
using Clifton.Core.Pipes;
using System.Threading.Tasks;

namespace hyper.Inputs
{
    [Target("ConsoleInput")]
    public sealed class ConsoleInput : TargetWithLayout, IInput
    {


        public bool CanRead { get; set; } = false;

        string currentMessage = "";

        Task backgroundTask = null;

        public event ConsoleCancelEventHandler CancelKeyPress;

        public ConsoleInput()
        {
            Name = "ConsoleInput";
            backgroundTask = new Task(() =>
            {
                while (true)
                {
                    var message = Console.ReadLine();
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


            });
            backgroundTask.Start();
        }


        public bool Available()
        {
            return currentMessage?.Length > 0;
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

            
               Console.WriteLine(logMessage);

            
        }

    }
}
