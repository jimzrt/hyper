using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace hyper.Inputs
{
    public interface IInput
    {
        public event ConsoleCancelEventHandler CancelKeyPress;

        //   public bool CanRead { get; set; }

        // public bool Available();

        // public void Flush();

        //    public string Read();

        // void SetResetEvent(ManualResetEvent resetEvent);

        void Interrupt();

        void SetQueue(BlockingCollection<string> ownQueue);
    }
}