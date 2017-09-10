using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Data;
using Core.Repositories;
using Core.Services;
using Microsoft.Extensions.Logging;

namespace Core.BusinessLogic
{
    public enum PriceUpDown
    {
        Down = 0,
        Up = 1
    }


    public class PriceChange
    {
        public readonly double PercentageChanged;
        public readonly string HumanReadableString;
        public readonly PriceUpDown PriceUpDown;
        public readonly string MarketLiteral;

        public PriceChange(double percentageChanged, PriceUpDown priceUpDown, string humanReadableString, string marketLiteral)
        {
            PercentageChanged = percentageChanged;
            PriceUpDown = priceUpDown;
            HumanReadableString = humanReadableString;
            MarketLiteral = marketLiteral;
        }

        public override string ToString()
        {
            return $"{MarketLiteral} - {PercentageChanged} | {PriceUpDown}";
        }
    }
    public interface ICoreChartLogic
    {
        Task<PriceChange> GetCalculatedPriceChange(string marketLiteral, int minutes);
    }

    public class CoreChartLogic : ICoreChartLogic
    {
        private readonly ILogger<CoreChartLogic> _logger;
        private readonly IBittrexApiRepository _repository;
        private readonly IBittrexService _bittrexService;

        public CoreChartLogic(ILogger<CoreChartLogic> logger, IBittrexApiRepository repository, IBittrexService bittrexService)
        {
            _logger = logger;
            _repository = repository;
            _bittrexService = bittrexService;
        }

        public async Task<PriceChange> GetCalculatedPriceChange(string marketLiteral, int minutes)
        {
            _logger.LogInformation($"Starting to calculate price in main logic module: {marketLiteral} / mintues: {minutes}");
            var currentTicker = await _repository.GetTickerAsync(marketLiteral);
            var marketOrders = await _repository.GetMarketHistory(marketLiteral);

            var currentPrice = currentTicker.result.Last;
            var historicOrder =
                marketOrders.result.FirstOrDefault(x => x.TimeStamp == GetClosestDateTimeToMinutes(marketOrders.result.Select(t => t.TimeStamp), minutes));
            var historicPrice = historicOrder?.Price;

            var percentageChanged = GetPercentageChanged(historicPrice, (double)currentPrice);
            var priceDirection = (double)currentPrice > historicPrice ? PriceUpDown.Up : PriceUpDown.Down;
            var readableString = $"% {(decimal)(Math.Round(percentageChanged, 5) * 100)}";

            _logger.LogInformation($"Calculated price, values are %change: {percentageChanged}, price direction is {priceDirection}");
            return new PriceChange(percentageChanged, priceDirection, readableString, marketLiteral);
        }

        private double GetPercentageChanged(double? historicPrice, double? currentPrice)
        {
            /*TODO: Implement floating point comparison tolerance check! Also implement decimal comparison in method above ffs*/
            var change = currentPrice - historicPrice;
            if (change != null) return change.Value / currentPrice.Value;
            return 0;
        }

        /// <summary>
        /// Takes a list of DateTimes and returns the one closest to minutesAgo parameter provided.
        /// </summary>
        /// <param name="list">List of DateTimes to iterate over</param>
        /// <param name="minutesAgo">Integer provided for mintues ago</param>
        /// <returns>DateTime - Closest DateTime x minutes ago</returns>
        private DateTime GetClosestDateTimeToMinutes(IEnumerable<DateTime> list, int minutesAgo)
        {
            /*TODO Rework this, be very carefull! This is core*/
            var dateTimes = list.OrderBy(x => x.Ticks).ToList();
            var closestDate = new DateTime();
            var targetDate = DateTime.Now.AddHours(-3).AddMinutes(-minutesAgo);

            foreach (var currentDate in dateTimes)
            {
                if (currentDate >= targetDate)
                {
                    return currentDate;
                }
            }

            if (closestDate == DateTime.MinValue)
            {
                _logger.LogCritical($"Error when calculating GetClosestDateTimeToMinutes! date returned was {closestDate}");
                throw new Exception($"Something went very wrong, probably repositories are not updated!");
            }

            return new DateTime();
        }
    }
}
