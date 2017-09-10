using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Core.Data
{
    public interface ITickers : IApiResult<Ticker>
    {
        Ticker GetTicker(string marketLiteral);
        void AddTicker(Ticker ticker);
    }

    public class Tickers : ITickers
    {
        private readonly ILogger<Tickers> _logger;

        public bool success { get; set; }
        public string message { get; set; }
        public Dictionary<string, Ticker> tickers { get; set; }
        public Tickers(ILogger<Tickers> logger)
        {
            _logger = logger;
            tickers = new Dictionary<string, Ticker>();
            _logger.LogInformation($"{nameof(Tickers)} initialized!");
        }

        public IEnumerable<Ticker> GetApiResult()
        {
            throw new NotImplementedException();
        }

        public void SetResult(IEnumerable<Ticker> list)
        {
            throw new NotImplementedException();
        }

        public void AddTicker(Ticker ticker)
        {
            var result = tickers.ContainsKey(ticker.MarketLiteral);
            if (!result)
            {
                tickers.Add(ticker.MarketLiteral, ticker);
            }
        }

        public bool UpdateTicker(Ticker ticker)
        {
            if (tickers.ContainsKey(ticker.MarketLiteral))
            {
                tickers[ticker.MarketLiteral] = ticker;
                return true;
            }

            return false;
        }

        public Ticker GetTicker(string marketLiteral)
        {
            throw new NotImplementedException();
        }
    }

    public class Ticker
    {
        public string MarketLiteral { get; set; }
        [JsonProperty("success")]
        public bool success { get; set; }
        [JsonProperty("result")]
        public BidAskLastTriple result { get; set; }
    }

    public class BidAskLastTriple
    {
        [JsonProperty("Bid")]
        public decimal Bid { get; set; }
        [JsonProperty("Ask")]
        public decimal Ask { get; set; }
        [JsonProperty("Last")]
        public decimal Last { get; set; }
    }
}
