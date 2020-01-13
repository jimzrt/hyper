using NLog;
using NLog.Targets;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace hyper.Inputs
{
    [Target("ConsoleInput")]
    public sealed class ConsoleInput : TargetWithLayout, IInput
    {
        public bool CanRead { get; set; } = false;

        private string currentMessage = "";

        private Task backgroundTask = null;
        private ManualResetEvent resetEvent;

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
                    resetEvent.Set();
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

        public void SetResetEvent(ManualResetEvent resetEvent)
        {
            this.resetEvent = resetEvent;
        }

        public void Interrupt()
        {
            //TODO
            //stop console read
        }
    }
}