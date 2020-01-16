using hyper.Helper;
using LinqToDB.Mapping;
using System;

namespace hyper.Models
{
    [Table(Name = "hyper_events")]
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

        public override string ToString()
        {
            return $"Event: nId: {NodeId} - date: {Added} - type: {EventType} - value: {Value}";
        }
    }
}