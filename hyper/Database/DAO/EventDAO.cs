using hyper.Models;
using LinqToDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hyper.Database.DAO
{
    class EventDAO
    {

        public static void InsertEvent(Event _event)
        {
            using (var db = new DataConnection())
            {
                db.Insert(_event);
            }
        }

        public static async void InsertEventAsync(Event _event)
        {
            using (var db = new DataConnection())
            {
                await db.InsertAsync(_event);
            }
        }

        public static void InsertEvents(IEnumerable<Event> _events)
        {
            using (var db = new DataConnection())
            {
                foreach (var evt in _events)
                {
                    db.Insert(evt);
                }
            }
        }

        public static async Task InsertEventsAsync(IEnumerable<Event> _events)
        {

            await Task.Run(() => {

                using (var db = new DataConnection())
                {
                    foreach (var evt in _events)
                    {
                         db.Insert(evt);
                    }
                }

            });


        }

        public static Event GetFirst()
        {
            using (var db = new DataConnection())
            {
                return db.Event.First();
            }
        }

        public static IEnumerable<Event> GetAll()
        {
            using (var db = new DataConnection())
            {
                //var events =  from e in db.Event
                //              select e;
                
                return db.Event.ToList();
            }
        }
    }
}
