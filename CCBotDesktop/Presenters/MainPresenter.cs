using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.APIs;
using Core.Data;
using Core.DataBase;
using Microsoft.Extensions.Logging;

namespace CCBotDesktop.Presenters
{
    public interface IMainPresenter
    {
        Task<IEnumerable<Market>> GetMarketResult();
        IEnumerable<MarketPairTuple> GetMarketPairs();
        IEnumerable<Market> GetFavoriteMarkets();
    }

    public class MainPresenter : IMainPresenter
    {
        private ILogger<MainPresenter> _logger;
        private readonly ISqlLiteRepository _repository;
        private readonly IBittrexApi _bittrexApi;
        private readonly ICryptoWatchApi _cryptoWatchApi;
        private readonly IMarkets _markets;

        public MainPresenter(ILogger<MainPresenter> logger, ISqlLiteRepository repository, IBittrexApi bittrexApi, ICryptoWatchApi cryptoWatchApi, IMarkets markets)
        {
            _logger = logger;
            _repository = repository;
            _bittrexApi = bittrexApi;
            _cryptoWatchApi = cryptoWatchApi;
            _markets = markets;
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
    }
}
