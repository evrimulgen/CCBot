﻿using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows;
using CCBotDesktop.Presenters;
using Core.BusinessLogic;
using Core.Data;
using Core.Repositories;
using Core.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace CCBotDesktop
{
    /// <summary>
    /// ENTRY POINT OF APPLICATION!
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            var serviceProvider = serviceCollection.BuildServiceProvider();
            serviceProvider.GetService<ILoggerFactory>().AddSerilog();

            var presenter = serviceProvider.GetService<IMainPresenter>();
            presenter.Initialize();

            var startupWindow = new MainWindow(serviceProvider.GetService<ILogger<MainWindow>>(),
                    serviceProvider.GetService<IMainPresenter>());

            startupWindow.Show();
        }

        private static void ConfigureServices(IServiceCollection serviceCollection)
        {
            Log.Logger = new LoggerConfiguration()
              .Enrich.FromLogContext()
              .WriteTo.Console()
              .CreateLogger();

            serviceCollection.AddTransient<ICoreChartLogic, CoreChartLogic>();
            serviceCollection.AddTransient<ITickers, Tickers>();

            serviceCollection.AddSingleton<IBittrexService, BittrexService>();
            serviceCollection.AddSingleton<IMainPresenter, MainPresenter>();
            serviceCollection.AddSingleton<IBittrexApiRepository, BittrexApiRepository>();
            serviceCollection.AddSingleton<IMarkets, Markets>();
            serviceCollection.AddSingleton<ICurrencies, Currencies>();
            serviceCollection.AddTransient<IMarketOrders, MarketOrders>();
            serviceCollection.AddSingleton<IMarketsHistoryRepository, MarketsHistoryRepository>();
            serviceCollection.AddSingleton<IObservableRepository, ObservableRepository>();
            serviceCollection.AddLogging();
        }
    }
}
