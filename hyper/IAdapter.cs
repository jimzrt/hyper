using System;
using System.Collections.Generic;
using System.Text;

namespace hyper
{
    interface IAdapter
    {
        event EventHandler<string> CommandHandler;

        void Listen();

        void WriteLine(string line);

    }
}
