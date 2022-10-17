using LinqToDB.Configuration;
using LinqToDB.Data;
using LinqToDB.DataProvider.SqlServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StanDatabase
{
    public static class StanDatabaseConnection
    {
        public static DataConnection CreateDatabaseConnection()
        {
            string connectionString = GetConnectionString();

            // create options builder
            var builder = new LinqToDBConnectionOptionsBuilder();

            // configure connection string
            builder.UseMySql(connectionString);

            // pass configured options to data connection constructor
            var dataConnection = new DataConnection(builder.Build());

            return dataConnection;
        }

        private static string GetConnectionString()
        {
            // TODO
            // return "Server=.\;Database=Northwind;Trusted_Connection=True;Enlist=False";
            throw new NotImplementedException();
        }
    }
}
