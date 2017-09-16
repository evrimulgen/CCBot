using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

// ReSharper disable InconsistentNaming

namespace Core.Data
{
    public interface IMarketOrders
    {

    }

    [Obsolete ("Remove after implementing new API service")]
    public class MarketOrders : IMarketOrders
    {
        private readonly ILogger<MarketOrders> _logger;
        public bool success { get; set; }
        public string message { get; set; }
        public IEnumerable<MarketHistoryEntry> result { get; set; }

        public MarketOrders(ILogger<MarketOrders> logger)
        {
            _logger = logger;
            result = new List<MarketHistoryEntry>();
        }

        public IEnumerable<MarketHistoryEntry> GetApiResult()
        {
            return result;
        }

        public void SetResult(IEnumerable<MarketHistoryEntry> list)
        {
            result = list;
            _logger.LogInformation($"{nameof(MarketOrders)}: ApiResult set. Count: {result.Count()} ");
        }
    }

    public class MarketHistory
    {
        public string MarketLiteral { get; set; }
        [JsonProperty("success")]
        public bool Success { get; set; }
        [JsonProperty("message")]
        public string Message { get; set; }
        [JsonProperty("result")]
        public IEnumerable<MarketHistoryEntry> Result { get; set; }
    }

    public class MarketHistoryEntry
    {
        [JsonProperty("Id")]
        public int Id { get; set; }
        [JsonProperty("TimeStamp")]
        public DateTime TimeStamp { get; set; }
        [JsonProperty("Quantity")]
        public double Quantity { get; set; }
        [JsonProperty("Price")]
        public double Price { get; set; }
        [JsonProperty("Total")]
        public double Total { get; set; }
        [JsonProperty("FillType")]
        public string FillType { get; set; }
        [JsonProperty("OrderType")]
        public string OrderType { get; set; }
    }
}