using hyper.Models;
using LinqToDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZWave.CommandClasses;

namespace hyper.Database.DAO
{
    internal class EventDAO
    {

        private DataConnection db;

        public EventDAO()
        {
            db = new DataConnection();
        }

        public void InsertEvent(Event _event)
        {
            db.Insert(_event);
        }

        public async void InsertEventAsync(Event _event)
        {
            await db.InsertAsync(_event);
        }

        public void InsertEvents(IEnumerable<Event> _events)
        {
            foreach (var evt in _events)
            {
                db.Insert(evt);
            }
        }

        public async Task InsertEventsAsync(IEnumerable<Event> _events)
        {

            await Task.Run(() =>
            {

                foreach (var evt in _events)
                {
                    db.Insert(evt);
                }

            });


        }

        public  Event GetFirst()
        {
            return db.Event.First();
        }

        public IEnumerable<Event> GetAll()
        {
            //var events =  from e in db.Event
            //              select e;

            string type = typeof(COMMAND_CLASS_NOTIFICATION_V8.NOTIFICATION_REPORT).Name;
            var events = from e in db.Event
                         where e.EventType == type 
                         select e;
            return events.ToList();
        }

        public DateTime GetLastEvent(string type, int nodeId)
        {
            /*            var event = from e in db.Event
                                    select e;*/

            var lastEvent = (from e in db.Event
                              where e.EventType == type && e.NodeId == nodeId
                              orderby e.Added descending
                              select e).FirstOrDefault();

           return lastEvent?.Added ?? new DateTime();
        }
    }
}
