using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

// ReSharper disable InconsistentNaming

namespace Core.Data
{
    public interface IMarketOrders : IApiResult<HistoricOrder>
    {

    }

    public class MarketOrders : IMarketOrders
    {
        private readonly ILogger<MarketOrders> _logger;
        public bool success { get; set; }
        public string message { get; set; }
        public IEnumerable<HistoricOrder> result { get; set; }

        public MarketOrders(ILogger<MarketOrders> logger)
        {
            _logger = logger;
            result = new List<HistoricOrder>();
        }

        public IEnumerable<HistoricOrder> GetApiResult()
        {
            return result;
        }

        public void SetResult(IEnumerable<HistoricOrder> list)
        {
            result = list;
            _logger.LogInformation($"{nameof(MarketOrders)}: ApiResult set. Count: {result.Count()} ");
        }
    }

    public class HistoricOrder
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

// {
//	"success" : true,
//	"message" : "",
//	"result" : [{
//			"Id" : 319435,
//			"TimeStamp" : "2014-07-09T03:21:20.08",
//			"Quantity" : 0.30802438,
//			"Price" : 0.01263400,
//			"Total" : 0.00389158,
//			"FillType" : "FILL",
//			"OrderType" : "BUY"
//		}, {
//			"Id" : 319433,
//			"TimeStamp" : "2014-07-09T03:21:20.08",
//			"Quantity" : 0.31820814,
//			"Price" : 0.01262800,
//			"Total" : 0.00401833,
//			"FillType" : "PARTIAL_FILL",
//			"OrderType" : "BUY"
//		}, {
//			"Id" : 319379,
//			"TimeStamp" : "2014-07-09T02:58:48.127",
//			"Quantity" : 49.64643541,
//			"Price" : 0.01263200,
//			"Total" : 0.62713377,
//			"FillType" : "FILL",
//			"OrderType" : "SELL"
//		}, {
//			"Id" : 319378,
//			"TimeStamp" : "2014-07-09T02:58:46.27",
//			"Quantity" : 0.35356459,
//			"Price" : 0.01263200,
//			"Total" : 0.00446622,
//			"FillType" : "PARTIAL_FILL",
//			"OrderType" : "BUY"
//		}
//	]
//}
