using hyper.Helper;
using LinqToDB.Mapping;
using System;

namespace hyper.Models
{
    //public enum EventType
    //{
    //    [LinqToDB.Mapping.MapValue(Value = "BATTERY")]
    //    BATTERY,
    //    [LinqToDB.Mapping.MapValue(Value = "WAKEUP")]
    //    WAKEUP,
    //    [LinqToDB.Mapping.MapValue(Value = "NOTIFICATION")]
    //    NOTIFICATION,
    //    [LinqToDB.Mapping.MapValue(Value = "SENSOR_BINARY")]
    //    SENSOR_BINARY,
    //    [LinqToDB.Mapping.MapValue(Value = "BASIC_SET")]
    //    BASIC_SET,
    //    [LinqToDB.Mapping.MapValue(Value = "UNHANDLED")]
    //    UNHANDLED,
    //    [LinqToDB.Mapping.MapValue(Value = "SWITCH_BINARY_REPORT")]
    //    SWITCH_BINARY_REPORT,
    //    [LinqToDB.Mapping.MapValue(Value = "SENSOR_MULTILEVEL_REPORT")]
    //    SENSOR_MULTILEVEL_REPORT
    // }

    [Table(Name = "Events")]
    public class Event
    {
        public Event()
        {
            if (Added == default)
                Added = DateTime.Now;
        }

        [PrimaryKey, Identity]
        public int Id { get; set; }

        [Column(Name = "nodeId"), NotNull]
        public int NodeId { get; set; }

        [Column(Name = "eventType"), NotNull]
        public string EventType { get; set; }

        //[Column(Name = "key")]
        //public Enums.EventType EventTypeKey { get; set; }

        [Column(Name = "key")]
        public string EventTypeKey { get; private set; }

        public Enums.EventKey EventTypeKeyR
        {
            get { return (Enums.EventKey)Enum.Parse(typeof(Enums.EventKey), EventTypeKey); }
            set { EventTypeKey = value.ToString(); }
        }

        [Column(Name = "value")]
        public float? Value { get; set; }

        [Column(Name = "raw")]
        public string Raw { get; set; }

        [Column(Name = "added"), NotNull]
        public DateTime Added { get; set; }
    }
}