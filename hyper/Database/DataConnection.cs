using hyper.Models;
using LinqToDB;

namespace hyper.Database
{
    public class DataConnection : LinqToDB.Data.DataConnection
    {
        public DataConnection() : base("SQLite") { }

        public ITable<Event> Event => GetTable<Event>();

        // ... other tables ...
    }
}
