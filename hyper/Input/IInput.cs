using System;
using System.Collections.Generic;
using System.Text;

namespace hyper.Inputs
{
    interface IInput
    {
        public event ConsoleCancelEventHandler CancelKeyPress;
        public bool CanRead { get; set; }
        public bool Available();
        public void Flush();
        public string Read();
    }
}
