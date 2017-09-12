using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using CCBotDesktop.Presenters;
using Core.APIs;
using Core.BusinessLogic;
using Core.Data;
using Core.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CCBotDesktop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ILogger<MainWindow> _logger;
        private readonly IBittrexApi _bittrexApi;
        protected IMainPresenter Presenter { get; set; }
        protected DispatcherTimer SecondTimer { get; }

        public MainWindow(ILogger<MainWindow> logger, IMainPresenter presenter, IBittrexApi bittrexApi)
        {
            InitializeComponent();
            _logger = logger;
            _bittrexApi = bittrexApi;
            Presenter = presenter;
            SecondTimer = new DispatcherTimer();
        }

        private async void listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //TODO: Fix this
            if (MainList.SelectedItem == null)
            {
                _logger.LogError($"Selected item was null, probably sorting.");
                return;
            }

            var selected = MainList.SelectedItem?.ToString();
            var ticker = await Presenter.GetBittrexRepository().GetTickerAsync(selected);

            MarketNameTB.Text = ticker.MarketLiteral;
            BidTB.Text = ticker.Result.Bid.ToString(CultureInfo.CurrentCulture);
            AskTB.Text = ticker.Result.Ask.ToString(CultureInfo.CurrentCulture);
            LastTB.Text = ticker.Result.Last.ToString(CultureInfo.CurrentCulture);
            SetPercentageChangedValues();
        }

        private void Setup()
        {
            var markets = Presenter.GetBittrexRepository().GetMarkets().GetApiResult().OrderBy(x => x.BaseCurrency);

            if (markets.Any())
            {
                foreach (var market in markets)
                {
                    MainList.Items.Add(market.ToString());
                }

                FindPercentBTN.IsEnabled = true;
            }

            _logger.LogInformation($"Finished setup of MainWindow");

            MainListSelectBox.Items.Add("Order Alpha Asc");
            MainListSelectBox.Items.Add("Order Alpha Desc");
            //InitializeTimer();

            PercentageChangeMethodTB.Text = "5";
            MinutesBox.Text = "60";
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

        private void StartButton_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                Setup();
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"{nameof(MainWindow)} threw an exception!: {ex.Message}");
                _logger.LogDebug($"{nameof(MainWindow)} threw an exception!: {ex.StackTrace}");
            }
        }

        private void MainListSelectBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var value = MainListSelectBox.SelectedValue;
            var markets = Presenter.GetBittrexRepository().GetMarkets().GetApiResult();

            switch (value.ToString())
            {
                case
                    "Order Alpha Asc":
                    markets = markets.OrderBy(x => x.BaseCurrency).ToList();
                    break;
                case
                    "Order Alpha Desc":
                    markets = markets.OrderByDescending(x => x.BaseCurrency).ToList();
                    break;
                default:
                    break;
            }

            MainList.Items.Clear();

            if (markets.Any())
            {
                foreach (var market in markets)
                {
                    MainList.Items.Add(market.ToString());
                }
            }
        }

        private async void Debug_Click(object sender, RoutedEventArgs e)
        {
            await Presenter.GetBittrexRepository().FillTickers();

            var market = new Market()
            {
                BaseCurrency = "KEK",
                MarketCurrency = "BUR"
            };

            Presenter.GetObservableRepository().AddChangedMarket(market);
        }

        private async void SetPercentageChangedValues()
        {
            var currentlySelectedMarket = MainList.SelectedItem?.ToString();

            if (currentlySelectedMarket == null)
            {
                return;
            }

            int minutes;
            int.TryParse(MinutesBox.Text, out minutes);

            if (minutes == 0)
            {
                ErrorBox.Text = $"Could not parse minutes, please enter a value";
            }
            else
            {
                ErrorBox.Text = string.Empty;
            }

            var priceChange = await Presenter.GetMainLogic().GetCalculatedPriceChange(currentlySelectedMarket, minutes);

            PriceChangeTB.Text = priceChange.PriceUpDown.ToString();
            PercentageChangeTB.Text = priceChange.HumanReadableString;
        }

        private async Task<IEnumerable<string>> GetPcChanges(double percentageChanged)
        {
            if (percentageChanged == 0)
            {
                ErrorBox.Text = $"Please enter precentage threshold";
                return null;
            }
            var result = new List<PriceChange>();

            int minutes;
            int.TryParse(MinutesBox.Text, out minutes);

            if (minutes == 0)
            {
                ErrorBox.Text = $"Could not parse minutes, please enter a value";
                return Enumerable.Empty<string>();
            }
            else
            {
                ErrorBox.Text = string.Empty;
            }

            _logger.LogInformation($"Starting to fetch percentage changes!");
            FindPercentBTN.IsEnabled = false;

            try
            {
                foreach (var mainListSelectedItem in MainList.Items)
                {
                    if (string.IsNullOrWhiteSpace(mainListSelectedItem.ToString()))
                    {
                        continue;
                    }

                    var currentlySelectedMarket = mainListSelectedItem.ToString();
                    var priceChange = await Presenter.GetMainLogic()
                        .GetCalculatedPriceChange(currentlySelectedMarket, minutes);

                    if (priceChange.PercentageChanged * 100 > percentageChanged ||
                        priceChange.PercentageChanged * 100 < -percentageChanged)
                    {
                        _logger.LogInformation(
                            $"Pricetrend: {priceChange.PriceUpDown}, Pricechange: {priceChange.HumanReadableString}");
                        result.Add(priceChange);
                        ErrorBox.Text = $"FOUND: {priceChange.MarketLiteral} - {priceChange.PriceUpDown}";
                    }

                    DebugBox.Text = $"Comparing: {priceChange}";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"GetPcChanges threw an exception!: {ex.Message}");
                _logger.LogDebug($"GetPcChanges threw an exception!: {ex.StackTrace}");
            }
            finally
            {
                var list = result.OrderBy(x => x.PercentageChanged).ToList();
                PriceChangeListBox.Items.Clear();
                PriceChangeListBox.ItemsSource = null;

                foreach (var item in list)
                {
                    PriceChangeListBox.Items.Add(item);
                }

                _logger.LogInformation($"Finished fetcing changes!");
                ErrorBox.Text = $"Finished fetching percentages :)";
                DebugBox.Text = $"Done";
                FindPercentBTN.IsEnabled = true;
            }

            return null;
        }

        private async void GetPriceChangesOverTenPercent(object sender, RoutedEventArgs e)
        {
            double changed = 0;
            double.TryParse(PercentageChangeMethodTB.Text, out changed);
            try
            {
                await GetPcChanges(changed);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occured: {ex.Message}");
                _logger.LogError($"Error occured: {ex.StackTrace}");
            }
        }

        private void PriceChangeBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void CheckCoinigy_Click(object sender, RoutedEventArgs e)
        {
            var priceChangeDto = PriceChangeListBox.SelectedValue as PriceChange;
            if (priceChangeDto == null)
            {
                return;
            }

            var marketLiteral = priceChangeDto?.MarketLiteral;

            var markets = marketLiteral?.Split('-');
            var uriString = $"https://www.coinigy.com/main/markets/BTRX/{markets[1]}/{markets[0]}";

            Process.Start($"microsoft-edge:{uriString}");
        }

        private async void button1_Click(object sender, RoutedEventArgs e)
        {
            var sw = new Stopwatch();
            sw.Start();

            var testListMarkets = await _bittrexApi.GetMarkets();
            var testListCurrencies = await _bittrexApi.GetCurrencies();
            var testTicker = await _bittrexApi.GetTicker("BTC-LTC");
            var testOrderBook = await _bittrexApi.GetOrderbook("BTC-LTC", OrderbookType.Both);
            var testMarketHistories = await _bittrexApi.GetMarketHistory("BTC-LTC");
            var testMarketSummaries = await _bittrexApi.GetMarketSummaries();
            var testMarketSummary = await _bittrexApi.GetMarketSummary("BTC-LTC");


            //FillAllTickers
            //var tickerList = new List<Ticker>();
            //foreach (var literal in testListMarkets)
            //{
            //    tickerList.Add(await _bittrexApi.GetTicker($"{literal.BaseCurrency}-{literal.MarketCurrency}"));
            //}

            

            if (testListMarkets.IsNullOrEmpty() || testListCurrencies.Result.IsNullOrEmpty() || !testTicker.Success ||
                testOrderBook.BuyOrders.IsNullOrEmpty() || testOrderBook.SellOrders.IsNullOrEmpty() || testMarketHistories.Result.IsNullOrEmpty()
                || testMarketSummaries.Result.IsNullOrEmpty() || !(testMarketSummary.Result.First().Last > 0))
            {
                _logger.LogCritical($"A list was empty, check every list !");
            }

            sw.Stop();
            _logger.LogCritical($"Elapsed seconds: {(double) sw.ElapsedMilliseconds / 1000}");
        }
    }
} 
