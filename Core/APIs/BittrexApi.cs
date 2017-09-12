using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Core.Data;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Core.APIs
{
    public enum OrderbookType
    {
        Buy,
        Sell,
        Both
    }
    public interface IBittrexApi
    {
        Task<IEnumerable<Market>> GetMarkets();
        Task<Currencies> GetCurrencies();
        Task<Ticker> GetTicker(string marketLiteral);
        Task<Orders> GetOrderbook(string marketLiteral, OrderbookType type);
        Task<MarketHistory> GetMarketHistory(string marketLiteral);
        Task<MarketSummaries> GetMarketSummaries();
        Task<MarketSummarySingle> GetMarketSummary(string marketLiteral);
    }

    public class BittrexApi : IBittrexApi
    {
        private readonly ILogger<BittrexApi> _logger;
        private readonly HttpClient _reusableHttpClient;

        public BittrexApi(ILogger<BittrexApi> logger)
        {
            _logger = logger;
            _reusableHttpClient = new HttpClient();
            _reusableHttpClient.DefaultRequestHeaders.Accept.Clear();
            _reusableHttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<IEnumerable<Market>> GetMarkets()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var json = await client.GetStringAsync(new Uri($"https://bittrex.com/api/v1.1/public/getmarkets"));
                    var markets = JsonConvert.DeserializeObject<MarketsResult>(json);

                    if (markets.Success)
                    {
                        _logger.LogInformation($"Successfully fetched Markets from Bittrex");
                        return markets.Result;
                    }

                    _logger.LogError($"Fillmarkets API call returned false on success!");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"GetMarketsJson threw an exception!: {ex.Message}");
                _logger.LogDebug($"GetMarketsJson threw an exception!: {ex.StackTrace}");
            }

            return null;
        }

        public async Task<Currencies> GetCurrencies()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var json = await client.GetStringAsync(new Uri($"https://bittrex.com/api/v1.1/public/getcurrencies"));
                    var currencies = JsonConvert.DeserializeObject<Currencies>(json);

                    if (currencies.Success)
                    {
                        _logger.LogInformation($"Successfully fetched Currencies from Bittrex");
                        return currencies;
                    }

                    _logger.LogError($"GetCurrencies API call returned false on success!");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"GetCurrencies threw an exception!: {ex.Message}");
                _logger.LogDebug($"GetCurrencies threw an exception!: {ex.StackTrace}");
            }

            return null;
        }

        public async Task<Ticker> GetTicker(string marketLiteral)
        {
            try
            {
                var json = await _reusableHttpClient.GetStringAsync(new Uri($"https://bittrex.com/api/v1.1/public/getticker?market=" + $"{marketLiteral}"));
                var ticker = JsonConvert.DeserializeObject<Ticker>(json);

                if (ticker.Success)
                {
                    _logger.LogInformation($"Successfully fetched Ticker from Bittrex");
                    ticker.MarketLiteral = marketLiteral;
                    return ticker;
                }

                _logger.LogError($"GetTicker API call returned false on success!");
            }
            catch (Exception ex)
            {
                _logger.LogError($"GetTicker threw an exception!: {ex.Message}");
                _logger.LogDebug($"GetTicker threw an exception!: {ex.StackTrace}");
            }

            return null;
        }

        public async Task<Orders> GetOrderbook(string marketLiteral, OrderbookType type)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var uri =
                        $"https://bittrex.com/api/v1.1/public/getorderbook?market={marketLiteral}&type={type.ToString().ToLower()}";

                    var json = await client.GetStringAsync(new Uri(uri));
                    var orderBook = new OrderBook();

                    var jObject = JObject.Parse(json);
                    var resultBuy = (JArray)jObject["result"]["buy"];
                    var resultSell = (JArray)jObject["result"]["sell"];

                    foreach (var buy in resultBuy)
                    {
                        orderBook.Result.BuyOrders.Add(buy.ToObject<Buy>());
                    }

                    foreach (var sell in resultSell)
                    {
                        orderBook.Result.SellOrders.Add(sell.ToObject<Sell>());
                    }

                    orderBook.Success = jObject["success"].ToObject<bool>();

                    if (orderBook.Success)
                    {
                        _logger.LogInformation($"Successfully fetched OrderBook from Bittrex");
                        orderBook.MarketLiteral = marketLiteral;
                        return orderBook.Result;
                    }

                    _logger.LogError($"GetOrderbook API call returned false on success!");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"GetOrderbook threw an exception!: {ex.Message}");
                _logger.LogDebug($"GetOrderbook threw an exception!: {ex.StackTrace}");
            }

            return null;
        }

        public async Task<MarketHistory> GetMarketHistory(string marketLiteral)
        {
            try
            {
                var json = await _reusableHttpClient.GetStringAsync(new Uri($"https://bittrex.com/api/v1.1/public/getmarkethistory?market={marketLiteral}"));
                var marketHistoryEntries = JsonConvert.DeserializeObject<MarketHistory>(json);

                if (marketHistoryEntries.Success)
                {
                    marketHistoryEntries.MarketLiteral = marketLiteral;
                    _logger.LogInformation($"Successfully fetched MarketHistory from Bittrex");
                    return marketHistoryEntries;
                }

                _logger.LogError($"GetMarketHistory API call returned false on success!");
            }
            catch (Exception ex)
            {
                _logger.LogError($"GetMarketHistory threw an exception!: {ex.Message}");
                _logger.LogDebug($"GetMarketHistory threw an exception!: {ex.StackTrace}");
            }

            return null;
        }

        public async Task<MarketSummaries> GetMarketSummaries()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var json = await client.GetStringAsync(new Uri($"https://bittrex.com/api/v1.1/public/getmarketsummaries"));
                    var marketSummaries = JsonConvert.DeserializeObject<MarketSummaries>(json);

                    if (marketSummaries.Success)
                    {
                        _logger.LogInformation($"Successfully fetched MarketSummaries from Bittrex");
                        return marketSummaries;
                    }

                    _logger.LogError($"GetMarketSummaries API call returned false on success!");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"GetMarketSummaries threw an exception!: {ex.Message}");
                _logger.LogDebug($"GetMarketSummaries threw an exception!: {ex.StackTrace}");
            }

            return null;
        }

        public async Task<MarketSummarySingle> GetMarketSummary(string marketLiteral)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var json = await client.GetStringAsync(new Uri($"https://bittrex.com/api/v1.1/public/getmarketsummary?market={marketLiteral}"));
                    var marketSummary = JsonConvert.DeserializeObject<MarketSummarySingle>(json);

                    if (marketSummary.Success)
                    {
                        _logger.LogInformation($"Successfully fetched MarketSummary from Bittrex");
                        return marketSummary;
                    }

                    _logger.LogError($"GetMarketSummary API call returned false on success!");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"GetMarketSummary threw an exception!: {ex.Message}");
                _logger.LogDebug($"GetMarketSummary threw an exception!: {ex.StackTrace}");
            }

            return null;
        }
    }
}
