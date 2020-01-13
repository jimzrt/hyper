using System;
using System.Threading;

namespace hyper.Inputs
{
    internal interface IInput
    {
        public event ConsoleCancelEventHandler CancelKeyPress;

        public bool CanRead { get; set; }

        public bool Available();

        public void Flush();

        public string Read();

        void SetResetEvent(ManualResetEvent resetEvent);
        void Interrupt();
    }
}