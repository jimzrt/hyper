using LinqToDB.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace hyper.Database
{
    internal class DatabaseSettingsMYSQL : ILinqToDBSettings
    {
        public IEnumerable<IDataProviderSettings> DataProviders => Enumerable.Empty<IDataProviderSettings>();

        public string DefaultConfiguration => "SQLite";
        public string DefaultDataProvider => "MySql.Data.MySqlClient";

        public IEnumerable<IConnectionStringSettings> ConnectionStrings
        {
            get
            {
                yield return
                    new ConnectionStringSettings
                    {
                        Name = "SQLite",
                        ProviderName = "MySql.Data.MySqlClient",
                        ConnectionString = @"Server=localhost;Port=3306;Database=alfred;Uid=root;Pwd=keins001;charset=utf8;"
                    };
            }
        }
    }
}