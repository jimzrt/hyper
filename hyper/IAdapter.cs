using System;

namespace hyper
{
    internal interface IAdapter
    {
        event EventHandler<string> CommandHandler;

        void Listen();

        void WriteLine(string line);

    }
}
