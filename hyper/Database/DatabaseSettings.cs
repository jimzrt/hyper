﻿using LinqToDB.Configuration;
using System.Collections.Generic;
using System.Linq;

namespace hyper.Database
{
    public class DatabaseSettings : ILinqToDBSettings
    {
        public IEnumerable<IDataProviderSettings> DataProviders => Enumerable.Empty<IDataProviderSettings>();

        public string DefaultConfiguration => "SQLite";
        public string DefaultDataProvider => "System.Data.SQLite";

        public IEnumerable<IConnectionStringSettings> ConnectionStrings
        {
            get
            {
                yield return
                    new ConnectionStringSettings
                    {
                        Name = "SQLite",
                        ProviderName = "System.Data.SQLite",
                        ConnectionString = @"Data Source=logs/events.db;"
                    };
            }
        }
    }
}