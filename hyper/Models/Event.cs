using LinqToDB.Mapping;
using System;

namespace hyper.Models
{
    public enum EventType
    {
        [LinqToDB.Mapping.MapValue(Value = "BATTERY")]
        BATTERY,
        [LinqToDB.Mapping.MapValue(Value = "WAKEUP")]
        WAKEUP,
        [LinqToDB.Mapping.MapValue(Value = "NOTIFICATION")]
        NOTIFICATION,
        [LinqToDB.Mapping.MapValue(Value = "SENSOR_BINARY")]
        SENSOR_BINARY
    }


    [Table(Name = "Events")]
    public class Event
    {
        public Event()
        {
            Added = DateTime.Now;
        }

        [PrimaryKey, Identity]
        public int Id { get; set; }

        [Column(Name = "nodeId"), NotNull]
        public int NodeId { get; set; }

        [Column(Name = "eventType"), NotNull]
        public EventType EventType { get; set; }

        [Column(Name = "value")]
        public int? Value { get; set; }

        [Column(Name = "added"), NotNull]
        public DateTime Added { get; }

        // ... other columns ...
    }
}
