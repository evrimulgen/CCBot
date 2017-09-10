using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.BusinessLogic;
using Core.Data;
using Core.Repositories;
using Microsoft.Extensions.Logging;

namespace CCBotDesktop.Presenters
{
    public interface IMainPresenter
    {
        void Initialize();
        IBittrexApiRepository GetBittrexRepository();
        IObservableRepository GetObservableRepository();
        ICoreChartLogic GetMainLogic();
    }

    public class MainPresenter : IMainPresenter
    {
        public ILogger<MainPresenter> Logger;
        public IBittrexApiRepository BittrexApiRepo;
        public IObservableRepository ObservableRepository;
        private readonly ICoreChartLogic _mainLogic;

        public MainPresenter(ILogger<MainPresenter> logger, IBittrexApiRepository bittrexApiRepo, IObservableRepository observableRepository, ICoreChartLogic mainLogic)
        {
            Logger = logger;
            BittrexApiRepo = bittrexApiRepo;
            ObservableRepository = observableRepository;
            _mainLogic = mainLogic;
            Logger.LogInformation($"Repository initialized");
        }

        public void Initialize()
        {
            BittrexApiRepo.FillRepository();
        }

        public IBittrexApiRepository GetBittrexRepository()
        {
            return BittrexApiRepo;
        }

        public IObservableRepository GetObservableRepository()
        {
            return ObservableRepository;
        }

        public ICoreChartLogic GetMainLogic()
        {
            return _mainLogic;
        }
    }
}
