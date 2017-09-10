﻿using System;
using System.Collections.Generic;
using System.Linq;
using Core.Extensions;
using Microsoft.Extensions.Logging;

// ReSharper disable InconsistentNaming

namespace Core.Data
{
    public interface IMarkets : IApiResult<Market>
    {
        IEnumerable<MarketPairTuple> GetMarketPairs();
    }

    public class Markets : IMarkets
    {
        private readonly ILogger<Markets> _logger;
        public bool success { get; set; }
        public string message { get; set; }
        public IEnumerable<Market> result { get; set; }
        public IList<MarketPairTuple> marketPairs { get; set; }

        public Markets(ILogger<Markets> logger)
        {
            _logger = logger;
            marketPairs = new List<MarketPairTuple>();
        }

        public void CreateMarketPairs()
        {
            marketPairs?.Clear();

            if (result.IsNullOrEmpty())
            {
                _logger.LogError($"Markets resultlist was empty or null. Be sure to fill the resultset before accessing it");
            }

            foreach (var market in result)
            {
                marketPairs?.Add(new MarketPairTuple()
                {
                    BaseCurrency = market.BaseCurrency,
                    MarketCurrency = market.MarketCurrency
                });
            }
        }

        public IEnumerable<MarketPairTuple> GetMarketPairs()
        {
            if (marketPairs.IsNullOrEmpty())
            {
                throw new ArgumentOutOfRangeException($"MarketPairs is null or empty, make sure to fill it befire calling this method!");
            }

            return marketPairs;
        }

        public void SetResult(IEnumerable<Market> list)
        {
            result = list;
            CreateMarketPairs();
            _logger.LogInformation($"{nameof(Markets)}: ApiResult set. Count: {result.Count()} ");
        }

        public IEnumerable<Market> GetApiResult()
        {
            return result;
        }
    }

    public class MarketPairTuple
    {
        public string BaseCurrency { get; set; }
        public string MarketCurrency { get; set; }

        public string GetMarketLiteralString()
        {
            return ToString();
        }

        public override string ToString()
        {
            return $"{BaseCurrency}-{MarketCurrency}";
        }
    }

    public class Market
    {
        public string MarketCurrency { get; set; }
        public string BaseCurrency { get; set; }
        public string MarketCurrencyLong { get; set; }
        public string BaseCurrencyLong { get; set; }
        public double MinTradeSize { get; set; }
        public string MarketName { get; set; }
        public bool IsActive { get; set; }
        public DateTime Created { get; set; }

        public override string ToString()
        {
            return $"{MarketName}";
        }
    }
}


//{
//	"success" : true,
//	"message" : "",
//	"result" : [{
//			"MarketCurrency" : "LTC",
//			"BaseCurrency" : "BTC",
//			"MarketCurrencyLong" : "Litecoin",
//			"BaseCurrencyLong" : "Bitcoin",
//			"MinTradeSize" : 0.01000000,
//			"MarketName" : "BTC-LTC",
//			"IsActive" : true,
//			"Created" : "2014-02-13T00:00:00"
//		}, {
//			"MarketCurrency" : "DOGE",
//			"BaseCurrency" : "BTC",
//			"MarketCurrencyLong" : "Dogecoin",
//			"BaseCurrencyLong" : "Bitcoin",
//			"MinTradeSize" : 100.00000000,
//			"MarketName" : "BTC-DOGE",
//			"IsActive" : true,
//			"Created" : "2014-02-13T00:00:00"
//		}
//    ]
//}
