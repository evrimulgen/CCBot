using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace Core.Data
{
    public interface IMarketsHistoryRepository
    {
        bool AddOrdersToMarket(string marketName, MarketOrders orderBook);
        IList<MarketHistoryEntry> GetAllOrdersForMarket(string marketName);
        IDictionary<string, IList<MarketHistoryEntry>> GetHistoricMarketOrders();
    }

    public class MarketsHistoryRepository : IMarketsHistoryRepository
    {
        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly ILogger<MarketsHistoryRepository> _logger;
        public IDictionary<string, IList<MarketHistoryEntry>> HistoricMarketOrders { get; set; }
        public MarketsHistoryRepository(ILogger<MarketsHistoryRepository> logger)
        {
            _logger = logger;
            _logger.LogInformation($"MarketsHistoryRepository initialized");
        }

        public bool AddOrdersToMarket(string marketName, MarketOrders orderBook)
        {
            try
            {
                HistoricMarketOrders.Add(marketName, orderBook.GetApiResult().ToList());
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Adding MarketOrders to Dictionary failed at AddOrdersToMarket");
                _logger.LogError($"{ex.StackTrace}");
            }

            return false;
        }

        public IList<MarketHistoryEntry> GetAllOrdersForMarket(string marketName)
        {
            return HistoricMarketOrders[marketName];
        }

        public IDictionary<string, IList<MarketHistoryEntry>> GetHistoricMarketOrders()
        {
            return HistoricMarketOrders;
        }
    }
}
