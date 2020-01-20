using NLog;
using NLog.Targets;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace hyper.Inputs
{
    [Target("ConsoleInput")]
    public sealed class ConsoleInput : TargetWithLayout, IInput
    {
        // public bool CanRead { get; set; } = false;

        // private string currentMessage = "";
        private readonly object _syncObj = new object();

        private Queue<string> messageQueue = new Queue<string>();

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
                    if (message?.Trim().Length > 0)
                    {
                        if (message == "stop")
                        {
                            CancelKeyPress?.Invoke(null, null);
                            continue;
                        }
                        lock (_syncObj)
                            messageQueue.Enqueue(message);
                        resetEvent.Set();
                    }
                    //if (CanRead)
                    //{
                    //    currentMessage = message;
                    //}
                    //else
                    //{
                    //    if (message == "stop")
                    //    {
                    //        CancelKeyPress?.Invoke(null, null);
                    //    }
                    //}
                }
            });
            backgroundTask.Start();
        }

        public bool Available()
        {
            lock (_syncObj)
                return messageQueue.Count > 0;
        }

        public string Read()
        {
            lock (_syncObj)
                return messageQueue.Dequeue();
        }

        //public void Flush()
        //{
        //    currentMessage = "";
        //}

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