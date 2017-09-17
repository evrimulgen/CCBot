﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.APIs;
using Core.Data;
using Core.DataBase;
using Core.Extensions;
using Core.OhlcAnalysis;
using Microsoft.Extensions.Logging;

namespace CCBotDesktop.Presenters
{
    public interface IMainPresenter
    {
        Task<IEnumerable<Market>> GetMarketResult();
        IEnumerable<MarketPairTuple> GetMarketPairs();
        IEnumerable<Market> GetFavoriteMarkets();
        Task<double> GetSimpleMovingAverageForMarket(string btrxLiteral, int startSecondsAgo, int candleTimeInterval, int periods);
        Task<CryptoWatchMarketPairData> GetCryptoWatchPairs();
        ISqlLiteRepository Repository();
        IEnumerable<Market> GetBottableMarkets();
        Task<IEnumerable<CryptoWatchMarket>> GetCryptoWatchMarkets();
    }

    public class MainPresenter : IMainPresenter
    {
        private ILogger<MainPresenter> _logger;
        private readonly ISqlLiteRepository _repository;
        private readonly IBittrexApi _bittrexApi;
        private readonly ICryptoWatchApi _cryptoWatchApi;
        private readonly IMarkets _markets;
        private readonly IOhlcProcessor _processor;

        public MainPresenter(ILogger<MainPresenter> logger,
            ISqlLiteRepository repository,
            IBittrexApi bittrexApi,
            ICryptoWatchApi cryptoWatchApi,
            IMarkets markets,
            IOhlcProcessor processor)
        {
            _logger = logger;
            _repository = repository;
            _bittrexApi = bittrexApi;
            _cryptoWatchApi = cryptoWatchApi;
            _markets = markets;
            _processor = processor;
        }

        public async Task<IEnumerable<Market>> GetMarketResult()
        {
            var marketsList = await _bittrexApi.GetMarkets();
            return marketsList.ToList();
        }

        public IEnumerable<MarketPairTuple> GetMarketPairs()
        {
            return _markets.GetMarketPairs();
        }

        public IEnumerable<Market> GetFavoriteMarkets()
        {
            var marketLiterals = _repository.GetFavouriteBtrxLiterals();
            return marketLiterals.Select(lit => _markets.GetMarketFromBtrxLiteral(lit)).ToList();
        }

        public IEnumerable<Market> GetBottableMarkets()
        {
            var bottableMarketLiterals = _repository.GetBottableMarketLiterals();
            var returnList = new List<Market>();

            foreach (var marketLiteral in bottableMarketLiterals)
            {
                returnList.Add(_markets.GetMarketFromBtrxLiteral(marketLiteral));
            }

            return returnList;
        }

        public async Task<IEnumerable<CryptoWatchMarket>> GetCryptoWatchMarkets()
        {
            var cryptoMarkets = await _cryptoWatchApi.GetMarkets();
            return cryptoMarkets.Result;
        }

        public async Task<double> GetSimpleMovingAverageForMarket(string btrxLiteral, int startSecondsAgo, int candleTimeInterval, int periods)
        {
            var unixTimeStart = DateTime.Now.ConvertDateTimeToUnixAndSubstractSeconds(startSecondsAgo);

            var candleDataTask = await _cryptoWatchApi.GetCandleData(
                btrxLiteral,
                OlhcBeforeAfterParam.After,
                unixTimeStart,
                new List<int>() {candleTimeInterval}
            );

            await Task.Delay(500);

            var result = _processor.GetMovingAverage(candleDataTask.ResultSet.Values.First());
            return result;
        }

        public async Task<CryptoWatchMarketPairData> GetCryptoWatchPairs()
        {
            return await _cryptoWatchApi.GetPairs();
        }

        public ISqlLiteRepository Repository()
        {
            return _repository;
        }
    }
}
