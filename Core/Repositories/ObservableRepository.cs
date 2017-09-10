using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Data;
using Microsoft.Extensions.Logging;

namespace Core.Repositories
{
    public interface IObservableRepository
    {
        ObservableCollection<Market> GetChangedMarkets();
        void AddChangedMarket(Market market);
    }

    public class ObservableRepository : IObservableRepository
    {
        private readonly ILogger<ObservableRepository> _logger;
        private ObservableCollection<Market> _changedMarkets;

        public ObservableRepository(ILogger<ObservableRepository> logger )
        {
            _logger = logger;
            _changedMarkets = new ObservableCollection<Market>();
        }

        public void AddChangedMarket(Market market)
        {
            _changedMarkets.Add(market);
        }

        public ObservableCollection<Market> GetChangedMarkets()
        {
            return _changedMarkets;
        }
    }
}
