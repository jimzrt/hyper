using System;

namespace hyper
{
    internal class EventFilter
    {
        public byte? NodeId { get; set; }
        public int? Count { get; set; }
        public string Command { get; set; }
    }
}