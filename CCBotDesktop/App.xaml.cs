using System.Windows;
using CCBotDesktop.Presenters;
using Core.APIs;
using Core.Data;
using Core.DataBase;
using Core.DataProcessor;
using Core.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace CCBotDesktop
{
    /// <inheritdoc />
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

            var startupWindow = new MainWindow(
                    serviceProvider.GetService<ILogger<MainWindow>>(),
                    serviceProvider.GetService<IMainPresenter>(), 
                    serviceProvider.GetService<ICandleStickProcessor>(), 
                    serviceProvider.GetService<IBittrexApi>(),
                    serviceProvider.GetService<ICryptoWatchApi>(),
                    serviceProvider.GetService<ISqlLiteRepository>());

            startupWindow.Show();
        }

        private static void ConfigureServices(IServiceCollection serviceCollection)
        {
            Log.Logger = new LoggerConfiguration()
              .Enrich.FromLogContext()
              .WriteTo.Console()
              .CreateLogger();

            serviceCollection.AddSingleton<ICryptoWatchApi, CryptoWatchApi>();
            serviceCollection.AddSingleton<IBittrexService, BittrexService>();
            serviceCollection.AddSingleton<IMainPresenter, MainPresenter>();
            serviceCollection.AddSingleton<IMarkets, MarketsResult>();
            serviceCollection.AddSingleton<ICurrencies, Currencies>();
            serviceCollection.AddSingleton<IBittrexApi, BittrexApi>();
            serviceCollection.AddTransient<ISqlLiteRepository, SqlLiteRepository>();
            serviceCollection.AddTransient<ICandleStickProcessor, CandleStickProcessor>();

            serviceCollection.AddLogging();
        }
    }
}
