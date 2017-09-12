using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Core.BusinessLogic;
using Core.Data;
using Core.Extensions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Core.Repositories
{
    public interface IBittrexApiRepository : IApiRepository
    {
        IMarkets GetMarkets();
        ICurrencies GetCurrencies();
        ITickers GetTickers();
        Task<MarketOrders> GetMarketHistory(string marketLiteral);
        Task<bool> FillTickers();
        Task<Ticker> GetTickerAsync(string marketLiteral);
    }

    public class BittrexApiRepository : IBittrexApiRepository
    {
        private readonly ILogger<BittrexApiRepository> _logger;
        private readonly IMarkets _markets;
        private readonly IMarketOrders _marketOrders;
        private readonly ICurrencies _currencies;
        private readonly IMarketsHistoryRepository _marketsHistoryRepository;
        private readonly ITickers _tickers;


        public BittrexApiRepository(ILogger<BittrexApiRepository> logger, IMarkets markets, IMarketOrders marketOrders, ICurrencies currencies,
            IMarketsHistoryRepository marketsHistoryRepo, ITickers tickers)
        {
            _logger = logger;
            _markets = markets;
            _marketOrders = marketOrders;
            _currencies = currencies;
            _marketsHistoryRepository = marketsHistoryRepo;
            _tickers = tickers;
        }

        public bool IsHealthy()
        {
            return !_markets.GetApiResult().IsNullOrEmpty() &&
                !_currencies.GetApiResult().IsNullOrEmpty() &&
                !_marketsHistoryRepository.GetHistoricMarketOrders().IsNullOrEmpty();
        }

        public async Task<bool> FillRepository()
        {
            var tasks = new List<Task<bool>>
            {
                FillMarkets(),
                FillCurrencies()
                /*TODO: Implement method correctly*/
                //FillHistories()
            };

            await Task.WhenAll(tasks);
            return true;
        }

        public async Task<bool> FillTickers()
        {
            var marketLiterals = _markets.GetMarketPairs().ToList();
            foreach (var marketPairTuple in marketLiterals)
            {
                _tickers.AddTicker(await GetTickerAsync(marketPairTuple.GetMarketLiteralString()));
            }

            _logger.LogInformation($"Filled all initial Tickers!");
            return true;
        }

        public IMarkets GetMarkets()
        {
            return _markets;
        }

        public ICurrencies GetCurrencies()
        {
            return _currencies;
        }

        #region APILOGIC

        private async Task<bool> FillMarkets()
        {
            try
            {
                var json = await GetMarketsJson();
                var markets = JsonConvert.DeserializeObject<MarketsResult>(json);

                if (markets.Success)
                {
                    _markets.SetResult(markets.GetApiResult());
                    _logger.LogInformation($"Successfully initialized markets");
                    return true;
                }

                _logger.LogError($"Fillmarkets API call returned false on success!");
            }
            catch (Exception ex)
            {
                _logger.LogError($"FillMarkets threw an exception!: {ex.Message}");
                _logger.LogDebug($"FillMarkets threw an exception!: {ex.StackTrace}");
            }

            return false;
        }

        private async Task<string> GetMarketsJson()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    return await client.GetStringAsync(new Uri($"https://bittrex.com/api/v1.1/public/getmarkets"));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"GetMarketsJson threw an exception!: {ex.Message}");
                _logger.LogDebug($"GetMarketsJson threw an exception!: {ex.StackTrace}");
            }

            return null;
        }

        public async Task<bool> FillCurrencies()
        {
            try
            {
                var json = await GetCurrenciesJson();
                var currencies = JsonConvert.DeserializeObject<Currencies>(json);

                if (currencies.Success)
                {
                    _currencies.SetResult(currencies.GetApiResult());
                    _logger.LogInformation($"Successfully initialized currencies");
                    return true;
                }

                _logger.LogError($"Fillcurrencies API call returned false on success!");
            }
            catch (Exception ex)
            {
                _logger.LogError($"FillCurrencies threw an exception!: {ex.Message}");
                _logger.LogDebug($"FillCurrencies threw an exception!: {ex.StackTrace}");
            }

            return false;
        }

        private async Task<string> GetCurrenciesJson()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    return await client.GetStringAsync(new Uri("https://bittrex.com/api/v1.1/public/getcurrencies"));

                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"GetCurrenciesJson threw an exception!: {ex.Message}");
                _logger.LogDebug($"GetCurrenciesJson threw an exception!: {ex.StackTrace}");
            }

            return null;
        }

        private async Task<string> GetMarketHistoryJson(string marketLiteral)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    return await client.GetStringAsync(new Uri($"https://bittrex.com/api/v1.1/public/getmarkethistory?market=" + $"{marketLiteral}"));

                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"GetCurrenciesJson threw an exception!: {ex.Message}");
                _logger.LogDebug($"GetCurrenciesJson threw an exception!: {ex.StackTrace}");
            }

            return null;
        }

        public async Task<MarketOrders> GetMarketHistory(string marketLiteral)
        {
            try
            {
                var json = await GetMarketHistoryJson(marketLiteral);
                var history = JsonConvert.DeserializeObject<MarketOrders>(json);

                if (history.success)
                {
                    _marketOrders.SetResult(history.GetApiResult());
                    _logger.LogInformation($"Successfully addet history {history.success}, Count: {history.GetApiResult().Count()}");
                    return (MarketOrders)_marketOrders;
                }

                _logger.LogError($"Fillmarkets API call returned false on success!");
            }

            catch (Exception ex)
            {
                _logger.LogError($"GetMarketHistory threw an exception!: {ex.Message}");
                _logger.LogDebug($"GetMarketHistory threw an exception!: {ex.StackTrace}");
            }

            return null;
        }

        public async Task<Ticker> GetTickerAsync(string marketLiteral)
        {
            try
            {
                var json = await GetTickerJson(marketLiteral);
                var ticker = JsonConvert.DeserializeObject<Ticker>(json);

                if (ticker.Success)
                {
                    ticker.MarketLiteral = marketLiteral;
                    _tickers.AddTicker(ticker);
                    _logger.LogInformation($"Successfully addet ticker {ticker.MarketLiteral}");
                    return ticker;
                }

                _logger.LogError($"Fillmarkets API call returned false on success!");
            }
            catch (Exception ex)
            {
                _logger.LogError($"FillMarkets threw an exception!: {ex.Message}");
                _logger.LogDebug($"FillMarkets threw an exception!: {ex.StackTrace}");
            }

            return null;
        }

        private async Task<string> GetTickerJson(string marketLiteral)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    return await client.GetStringAsync(new Uri($"https://bittrex.com/api/v1.1/public/getticker?market=" + $"{marketLiteral}"));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"GetTickerJson threw an exception!: {ex.Message}");
                _logger.LogDebug($"GetTickerJson threw an exception!: {ex.StackTrace}");
            }

            return null;
        }

        public ITickers GetTickers()
        {
            return _tickers;
        }

        #endregion
    }
}
