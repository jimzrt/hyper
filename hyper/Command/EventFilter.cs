using System;

namespace hyper
{
    internal class EventFilter
    {
        public byte NodeId { get; set; } = 0;
        public int Count { get; set; } = 0;
        public string Command { get; set; } = "all";
    }
}