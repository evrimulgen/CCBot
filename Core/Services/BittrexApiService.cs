using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Core.Data;
using Core.Repositories;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Core.Services
{
    public interface IBittrexService
    {
        void DoMinuteTasks();
    }

    public class BittrexService : IBittrexService
    {
        private readonly ILogger<BittrexService> _logger;
        private readonly IBittrexApiRepository _bittrexApiRepository;

        public BittrexService(ILogger<BittrexService> logger, IBittrexApiRepository bittrexApiRepo)
        {
            _logger = logger;
            _bittrexApiRepository = bittrexApiRepo;
        }

        public void Initialize()
        {
            _logger.LogDebug("Initialized BittrexService");
        }

        public void DoMinuteTasks()
        {
            throw new NotImplementedException();
        }


        //public async Task<string> GetTickerAsync(string marketLiteral)
        //{
        //    try
        //    {
        //        var json = await GetTickerJson(marketLiteral);
        //        var tickers = JsonConvert.DeserializeObject<Currencies>(json).result;

        //        foreach (var ticker in tickers)
        //        {
        //            _logger.LogInformation((ticker.Currency + " / " + ticker.CoinType));
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError($"GetCurrencies threw an exception!: {ex.Message}");
        //        _logger.LogDebug($"GetCurrencies threw an exception!: {ex.StackTrace}");
        //    }

        //    return null;
        //}

        ////F.e: BTC-LTC
        //private async Task<string> GetTickerJson(string marketLiteral)
        //{
        //    try
        //    {
        //        using (var client = new HttpClient())
        //        {
        //            client.DefaultRequestHeaders.Accept.Clear();
        //            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        //            return await client.GetStringAsync(new Uri($"https://bittrex.com/api/v1.1/public/getticker?market=" + $"{marketLiteral}"));

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError($"GetCurrenciesJson threw an exception!: {ex.Message}");
        //        _logger.LogDebug($"GetCurrenciesJson threw an exception!: {ex.StackTrace}");
        //    }

        //    return null;
        //}

        //public async Task<IList<HistoricOrder>> GetMarketHistory(string marketLiteral)
        //{
        //    try
        //    {
        //        var json = await GetMarketHistoryJson(marketLiteral);
        //        var histories = JsonConvert.DeserializeObject<MarketOrders>(json).result;

        //        foreach (var history in histories)
        //        {
        //            _logger.LogInformation((history.Quantity + " / " + history.TimeStamp));
        //        }

        //        return histories;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError($"GetCurrencies threw an exception!: {ex.Message}");
        //        _logger.LogDebug($"GetCurrencies threw an exception!: {ex.StackTrace}");
        //    }

        //    return null;
        //}

        ////F.e: BTC-LTC

        
        //public async Task<IList<MarketPairTuple>> GetMarketPairTuples()
        //{
        //    if (!_markets.GetMarketPairs().Any())
        //    {
        //        await FillMarketsWithData();
        //    }

        //    return _markets.GetMarketPairs().ToList();
        //}

        //public async Task<IList<MarketDto>> GetAllMarkets()
        //{
        //    if (!_markets.GetMarkets().Any())
        //    {
        //        await FillMarketsWithData();
        //    }

        //    return _markets.GetMarkets().ToList();
        //}

        //public async Task<MarketDto> GetMarket(string marketLiteral)
        //{
        //    if (!_markets.GetMarkets().Any())
        //    {
        //        await FillMarketsWithData();
        //    }

        //    var kek = marketLiteral.Split('-');

        //    return
        //        _markets
        //            .GetMarkets().FirstOrDefault(mkt => (mkt.BaseCurrency == kek[0]) && (mkt.MarketCurrency == kek[1]));
        //}
    }
}
