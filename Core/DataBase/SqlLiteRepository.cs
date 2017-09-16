using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Diagnostics;
using Core.Extensions;
using Microsoft.Extensions.Logging;

namespace Core.DataBase
{
    public interface ISqlLiteRepository
    {
        IList<string> GetFavouriteBtrxLiterals();
        bool UpsertFavouriteBtrxLiteral(string marketLiteral);
    }

    public class SqlLiteRepository : ISqlLiteRepository
    {
        private readonly ILogger<SqlLiteRepository> _logger;
        private string ConnectionString { get; }

        public SqlLiteRepository(ILogger<SqlLiteRepository> logger)
        {
            _logger = logger;
            ConnectionString = $"Data Source=SqlLiteDb.db";
        }

        public IList<string> GetFavouriteBtrxLiterals()
        {
            try
            {
                var returnList = new List<string>();
                using (var conn = new SQLiteConnection(ConnectionString))
                {
                    conn.Open();

                    var command = conn.CreateCommand();
                    command.CommandText = "SELECT * FROM FAVORITE_MARKETS";
                    var dataReader = command.ExecuteReader();
                    var columnIndex = dataReader.GetOrdinal("BtrxLiteral");

                    while (dataReader.Read())
                    {
                        returnList.Add(dataReader.GetString(columnIndex));
                    }

                    dataReader.Close();
                }

                return returnList;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed at GetFavouriteBtrxLiterals, Message: {ex.Message}");
                _logger.LogDebug($"Failed at GetFavouriteBtrxLiterals, Stacktrace: {ex.StackTrace}");
                throw;
            }
        }

        public bool UpsertFavouriteBtrxLiteral(string marketLiteral)
        {
            try
            {
                using (var conn = new SQLiteConnection(ConnectionString))
                {
                    conn.Open();

                    var command = conn.CreateCommand();
                    command.CommandText =
                        $"IF NOT EXISTS (SELECT * FROM FAVORITE_MARKETS WHERE ID = '{marketLiteral}')" +
                        $"INSERT INTO FAVORITE_MARKETS(BtrxLiteral, CryptoWatchLiteral)" +
                        $"VALUES('{marketLiteral}','{marketLiteral.ConvertBittrexToCryptoWatchLiteral()}')" +
                        $"ELSE" +
                        $"UPDATE FAVORITE_MARKETS" +
                        $"SET BtrxLiteral = Val1, CryptoWatchLiteral = Val2" +
                        $"WHERE BtrxLiteral ='{marketLiteral}'";
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed at upserting {marketLiteral}, Message: {ex.Message}");
                _logger.LogDebug($"Failed at upserting {marketLiteral}, Stacktrace: {ex.StackTrace}");
                throw;
            }
        }

        public bool DeleteFavoriteBtrxLiteral(string marketLiteral)
        {
            try
            {
                using (var conn = new SQLiteConnection(ConnectionString))
                {
                    conn.Open();

                    var command = conn.CreateCommand();
                    command.CommandText =
                        $"delete from FAVORITE_MARKETS where BtrxLiteral = '{marketLiteral}'";
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed at upserting {marketLiteral}, Message: {ex.Message}");
                _logger.LogDebug($"Failed at upserting {marketLiteral}, Stacktrace: {ex.StackTrace}");
                throw;
            }
        }
    }
}
