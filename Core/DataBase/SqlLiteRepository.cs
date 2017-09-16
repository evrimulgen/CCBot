using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;

namespace Core.DataBase
{
    public interface ISqlLiteRepository
    {
        IList<string> GetFavouriteBtrxLiterals();
    }

    public class SqlLiteRepository : ISqlLiteRepository
    {
        public string ConnectionString { get; set; }

        public SqlLiteRepository()
        {
            
        }

        public IList<string> GetFavouriteBtrxLiterals()
        {
            ConnectionString = $"Data Source=clients.db;Version=3;New=False;Compress=True;";
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "select * from FavoriteMarkets";
                command.CommandType = CommandType.Text;

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var a = reader.NextResult();
                    }
                }
            }

            return null;
        }
    }
}
