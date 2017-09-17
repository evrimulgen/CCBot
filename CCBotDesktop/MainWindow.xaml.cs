using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using CCBotDesktop.Presenters;
using Core.APIs;
using Core.Data;
using Core.DataBase;
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
        protected IMainPresenter Presenter { get; set; }
        protected ObservableCollection<Market> ObservableMarketsList { get; set; }

        public MainWindow(ILogger<MainWindow> logger, IMainPresenter presenter)
        {
            InitializeComponent();
            _logger = logger;
            Presenter = presenter;
            Setup();
        }

        private void Setup()
        {
            ObservableMarketsList = new ObservableCollection<Market>();     
        }

        private async void DebugButton_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private async void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            ObservableMarketsList.Clear();
            var markets = await Presenter.GetMarketResult();
            markets.ToList().ForEach(x => ObservableMarketsList.Add(x));
            MarketsList.ItemsSource = ObservableMarketsList.OrderBy(x => x.BaseCurrency).ThenBy(x => x.MarketCurrency);
        }

        private void RadioButton_Checked_1(object sender, RoutedEventArgs e)
        {
            ObservableMarketsList.Clear();
            var favoriteMarkets = Presenter.GetFavoriteMarkets();
            favoriteMarkets.ToList().ForEach(x => ObservableMarketsList.Add(x));
            MarketsList.ItemsSource = ObservableMarketsList;
        }

        private async void SMAButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var marketLiteral = MarketsList.SelectedValue.ToString();
                var secondsAgoStart = int.Parse(MinutesBox.Text) * 60;
                var interValInSeconds = int.Parse(IntervalBox.Text) * 60;
                var periods = int.Parse(PeriodsBox.Text);

                var sma = await Presenter.GetSimpleMovingAverageForMarket(marketLiteral,
                    secondsAgoStart,
                    interValInSeconds,
                    periods);

                SimpleMovingAverageBox.Text = sma.ToString(CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                _logger.LogError($"SMAButton throw error: {ex.Message}");
                _logger.LogDebug($"SMAButton throw error: {ex.StackTrace}");
                SetUserMessage(ex.Message);
            }
            
        }

        private async void WriteMarketsToDatabase_Click(object sender, RoutedEventArgs e)
        {
            var cryptoPairs = await Presenter.GetCryptoWatchPairs();
            var btrxPairs = await Presenter.GetMarketResult();
            var markets = await Presenter.GetCryptoWatchMarkets();
            var marketTripleList = new List<MarketTriple>();

            var cryptoPairLiterals = cryptoPairs.ResultSet.Select(p => p.Id).OrderBy(p => p).ToList();
            var btrxPairLiterals = btrxPairs.Select(p => p.MarketName).OrderBy(p => p).ToList();
            var counter = 0;

            foreach (var btrxPairLiteral in btrxPairLiterals)
            {
                var cryptoLiteral = btrxPairLiteral.ConvertBittrexToCryptoWatchLiteral();
                if (cryptoPairLiterals.Contains(cryptoLiteral))
                {
                    var exchange = markets.Where(x => x.Pair == cryptoLiteral).Select(x => x.Exchange).FirstOrDefault();
                    marketTripleList.Add(new MarketTriple(btrxPairLiteral, cryptoLiteral, exchange));
                    _logger.LogDebug($"Fetched triple for DB: BTRX-Crypto-Exchange: {btrxPairLiteral} | {cryptoLiteral} | {exchange}");
                    counter++;
                }
            }
            try
            {
                Presenter.Repository().LoadAllMarketsIntoDb(marketTripleList);
            }
            catch (Exception ex)
            {
                _logger.LogError($"WriteMarketsToDatabase_Click throw error: {ex.Message}");
                _logger.LogDebug($"WriteMarketsToDatabase_Click throw error: {ex.StackTrace}");
                SetUserMessage(ex.Message);
            }
            
            SetUserMessage($"Wrote {counter} markets to Database");
        }

        private void SetUserMessage(string message)
        {
            UserMsgLabel.Content = message;
        }

        private void RadioButton_Checked_2(object sender, RoutedEventArgs e)
        {
            var bottableMarkets = Presenter.GetBottableMarkets();
            ObservableMarketsList.Clear();
            bottableMarkets.ToList().ForEach(x => ObservableMarketsList.Add(x));
            MarketsList.ItemsSource = ObservableMarketsList;
        }
    }
} 
