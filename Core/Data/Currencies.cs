using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Core.Data
{
    public interface ICurrencies : IApiResult<SingleCurrency>
    {
        
    }

    public class Currencies : ICurrencies
    {
        [JsonProperty("success")]
        public bool Success {get; set; }
        [JsonProperty("message")]
        public string Message { get; set; }
        [JsonProperty("result")]
        public IEnumerable<SingleCurrency> Result { get; set; }

        public IEnumerable<SingleCurrency> GetApiResult()
        {
            return Result;
        }

        public void SetResult(IEnumerable<SingleCurrency> list)
        {
            Result = list;
        }
    }
    public class SingleCurrency
    {
        public string CurrencyDto { get; set; }
        public string CurrencyLong { get; set; }
        public int MinConfirmation { get; set; }
        public double TxFee { get; set; }
        public bool IsActive { get; set; }
        public string CoinType { get; set; }
        public string BaseAddress { get; set; }
    }
}

//{
//	"success" : true,
//	"message" : "",
//	"result" : [{
//			"CurrencyDto" : "BTC",
//			"CurrencyLong" : "Bitcoin",
//			"MinConfirmation" : 2,
//			"TxFee" : 0.00020000,
//			"IsActive" : true,
//			"CoinType" : "BITCOIN",
//			"BaseAddress" : null
//		}, {
//			"CurrencyDto" : "LTC",
//			"CurrencyLong" : "Litecoin",
//			"MinConfirmation" : 5,
//			"TxFee" : 0.00200000,
//			"IsActive" : true,
//			"CoinType" : "BITCOIN",
//			"BaseAddress" : null
//		}
//    ]
//}
