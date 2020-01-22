using hyper.commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace hyper.Command
{
    public abstract class BaseCommand : ICommand
    {
        public byte NodeId { get; set; } = 0;

        public bool Retry { get; set; } = false;

        public abstract bool Start();

        public abstract void Stop();
    }
}