using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Diagnostics;
using System.Threading.Tasks;
using Core.Data;
using Core.Extensions;
using Microsoft.Extensions.Logging;

namespace Core.DataBase
{
    public interface ISqlLiteRepository
    {
        IList<string> GetFavouriteBtrxLiterals();
        IList<string> GetBottableMarketLiterals();
        bool UpsertFavouriteBtrxLiteral(string marketLiteral);
        bool DeleteFavoriteBtrxLiteral(string marketLiteral);
        bool LoadAllMarketsIntoDb(IEnumerable<MarketTriple> triples);
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

        public IList<string> GetBottableMarketLiterals()
        {
            try
            {
                var returnList = new List<string>();
                using (var conn = new SQLiteConnection(ConnectionString))
                {
                    conn.Open();

                    var command = conn.CreateCommand();
                    command.CommandText = "SELECT * FROM MARKETLITERALS";
                    var dataReader = command.ExecuteReader();
                    var columnIndex = dataReader.GetOrdinal("BtrxApiLiteral");

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
                _logger.LogError($"Failed at GetBottableMarketLiterals, Message: {ex.Message}");
                _logger.LogDebug($"Failed at GetBottableMarketLiterals, Stacktrace: {ex.StackTrace}");
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
                        $"INSERT INTO FAVORITE_MARKETS(BtrxLiteral, CryptoWatchLiteral)" +
                        $"VALUES('{marketLiteral}','{marketLiteral.ConvertBittrexToCryptoWatchLiteral()}')";

                    command.CommandType = CommandType.Text;
                    command.ExecuteNonQuery();
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

        public bool LoadAllMarketsIntoDb(IEnumerable<MarketTriple> triples)
        {
            try
            {
                foreach (var marketTriple in triples)
                {
                    UpsertMarketLiterals(marketTriple.BittrexLiteral, marketTriple.CryptoWatchLiteral, marketTriple.ExchangeName);
                }
                
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed at LoadAllMarketsIntoDb, Message: {ex.Message}");
                _logger.LogDebug($"Failed at LoadAllMarketsIntoDb, Stacktrace: {ex.StackTrace}");
                throw;
            }
        }

        private void UpsertMarketLiterals(string btrxLiteral, string cryptoLiteral, string exchangeName)
        {
            try
            {
                using (var conn = new SQLiteConnection(ConnectionString))
                {
                    conn.Open();

                    var command = conn.CreateCommand();
                    command.CommandText =
                        $"INSERT INTO MARKETLITERALS(BtrxApiLiteral, CryptoWatchApiLiteral, Exchange)" +
                        $"VALUES('{btrxLiteral}','{cryptoLiteral}', '{exchangeName}')";

                    command.CommandType = CommandType.Text;
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed at UpsertMarketLiterals BtrxLiteral: {btrxLiteral}, CryptoLiteral: {cryptoLiteral} Message: {ex.Message}");
                _logger.LogDebug($"Failed at UpsertMarketLiterals BtrxLiteral: {btrxLiteral}, CryptoLiteral: {cryptoLiteral} Stacktrace: {ex.StackTrace}");
                throw;
            }
        }
    }
}
