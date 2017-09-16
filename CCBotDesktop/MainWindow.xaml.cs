using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
            var markets = await Presenter.GetMarketResult();
            markets.ToList().ForEach(x => ObservableMarketsList.Add(x));
            MarketsList.ItemsSource = ObservableMarketsList;
        }

        private async void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            ObservableMarketsList.Clear();
            var markets = await Presenter.GetMarketResult();
            markets.ToList().ForEach(x => ObservableMarketsList.Add(x));
            MarketsList.ItemsSource = ObservableMarketsList;
        }

        private void RadioButton_Checked_1(object sender, RoutedEventArgs e)
        {
            ObservableMarketsList.Clear();
            var favoriteMarkets = Presenter.GetFavoriteMarkets();
            favoriteMarkets.ToList().ForEach(x => ObservableMarketsList.Add(x));
            MarketsList.ItemsSource = ObservableMarketsList;
        }
    }
} 
