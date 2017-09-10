using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;

// ReSharper disable InconsistentNaming

namespace Core.Data
{
    public interface ICurrencies : IApiResult<SingleCurrency>
    {
        
    }

    public class Currencies : ICurrencies
    {
        private readonly ILogger<Currencies> _logger;
        public bool success {get; set; }
        public string message { get; set; }
        public IEnumerable<SingleCurrency> result { get; set; }

        public Currencies(ILogger<Currencies> logger )
        {
            _logger = logger;
        }

        public IEnumerable<SingleCurrency> GetApiResult()
        {
            return result;
        }

        public void SetResult(IEnumerable<SingleCurrency> list)
        {
            result = list;
            _logger.LogInformation($"{nameof(Currencies)}: ApiResult set. Count: {result.Count()} ");
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
