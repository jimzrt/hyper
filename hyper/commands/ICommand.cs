using System;
using System.Collections.Generic;
using System.Text;

namespace hyper.commands
{
    interface ICommand
    {
        bool Active { get; }
        void Start();
        void Stop();

    }
}
