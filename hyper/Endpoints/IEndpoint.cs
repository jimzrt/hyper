using System;
using System.Collections.Generic;
using System.Text;

namespace hyper.Endpoints
{
    interface IEndpoint
    {
        public bool CanRead { get; set; }
        public bool Available();
        public void Flush();
        public string Read();
        void AddCancelEventHandler(Action<object, ConsoleCancelEventArgs> cancelHandler);
    }
}
