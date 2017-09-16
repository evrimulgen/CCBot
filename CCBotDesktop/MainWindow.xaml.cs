using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using CCBotDesktop.Presenters;
using Core.APIs;
using Core.DataBase;
using Core.DataProcessor;
using Core.Extensions;
using Microsoft.Extensions.Logging;

namespace CCBotDesktop
{
    /// <inheritdoc />
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly ILogger<MainWindow> _logger;
        private readonly ICandleStickProcessor _cdProcessor;
        private readonly IBittrexApi _bittrexApi;
        private readonly ICryptoWatchApi _cryptoWatchApi;
        private readonly ISqlLiteRepository _repository;
        protected IMainPresenter Presenter { get; set; }
        protected DispatcherTimer SecondTimer { get; }

        public MainWindow(ILogger<MainWindow> logger, 
            IMainPresenter presenter, 
            ICandleStickProcessor cdProcessor, 
            IBittrexApi bittrexApi, 
            ICryptoWatchApi cryptoWatchApi,
            ISqlLiteRepository repository)
        {
            InitializeComponent();
            _logger = logger;
            _cdProcessor = cdProcessor;
            _bittrexApi = bittrexApi;
            _cryptoWatchApi = cryptoWatchApi;
            _repository = repository;
            Presenter = presenter;
            SecondTimer = new DispatcherTimer();
        }

        private void InitializeTimer()
        {
            SecondTimer.Interval = TimeSpan.FromSeconds(3);
            SecondTimer.Tick += OneSecondTick;
            SecondTimer.Start();
        }

        private void OneSecondTick(object sender, EventArgs e)
        {
            _logger.LogInformation($"Timer running!  @ interval seconds: {SecondTimer.Interval}");
        }

        //All Testcode here
        private async void DebugButton_Click(object sender, RoutedEventArgs e)
        {
            var sw = new Stopwatch();
            sw.Start();

            //var testListMarkets = await _bittrexApi.GetMarkets();
            //var testListCurrencies = await _bittrexApi.GetCurrencies();
            //var testTicker = await _bittrexApi.GetTicker("BTC-LTC");
            //var testOrderBook = await _bittrexApi.GetOrderbook("BTC-LTC", OrderbookType.Both);
            //var testMarketHistories = await _bittrexApi.GetMarketHistory("BTC-LTC");
            //var testMarketSummaries = await _bittrexApi.GetMarketSummaries();
            //var testMarketSummary = await _bittrexApi.GetMarketSummary("BTC-LTC");

            //var testDataProcessorGetMarketChangePercentage = 
            //    await _cdProcessor.GetMarketChangePercentage("BTC-LTC", 3600, new List<int>() {60});

            var testSqlFavoriteMarkets = _repository.GetFavouriteBtrxLiterals();


            //if (testListMarkets.IsNullOrEmpty() || testListCurrencies.Result.IsNullOrEmpty() || !testTicker.Success ||
            //    testOrderBook.BuyOrders.IsNullOrEmpty() || testOrderBook.SellOrders.IsNullOrEmpty() || testMarketHistories.Result.IsNullOrEmpty()
            //    || testMarketSummaries.Result.IsNullOrEmpty() || !(testMarketSummary.Result.First().Last > 0))
            //{
            //    _logger.LogCritical($"A list was empty, check every list !");
            //    throw new Exception("Totaly fail Tests have failed. Implement a real test project!");
            //}

            sw.Stop();
            _logger.LogCritical($"Elapsed seconds: {(double) sw.ElapsedMilliseconds / 1000}");
        }
    }
} 
