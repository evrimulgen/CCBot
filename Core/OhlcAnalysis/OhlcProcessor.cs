using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Data;
using Microsoft.Extensions.Logging;

namespace Core.OhlcAnalysis
{
    public interface IOhlcProcessor
    {
        double GetMovingAverage(IEnumerable<CandleStick> candleSticks, int periods = 20);
        BollingerBands GetBollingerBands(IEnumerable<CandleStick> candleSticks, int periods);
    }

    public class OhlcProcessor : IOhlcProcessor
    {
        private readonly ILogger<OhlcProcessor> _logger;
        public OhlcProcessor(ILogger<OhlcProcessor> logger)
        {
            _logger = logger;
        }

        public double GetMovingAverage(IEnumerable<CandleStick> candleSticks, int periods = 20)
        {
            var enumerable = candleSticks as CandleStick[] ?? candleSticks.ToArray();
            return enumerable
                .OrderByDescending(cs => cs.CloseTime)
                .Take(periods)
                .Sum(cs => (double) cs.ClosePrice) / enumerable.Length;
        }

        public BollingerBands GetBollingerBands(IEnumerable<CandleStick> candleSticks, int periods)
        {
            try
            {
                var candleData = candleSticks.ToList();

                var sma20 = GetMovingAverage(candleData, periods);

                var last20ClosingTimes =
                    candleData.OrderByDescending(x => (double) x.ClosePrice)
                    .Take(20)
                    .ToList();

                var closePrices = last20ClosingTimes
                    .Select(x => x.ClosePrice - (decimal) sma20);

                var squaredValues = closePrices.Select(number => number > 0 ? 
                    Math.Sqrt((double) number) : 
                    Math.Sqrt((double) number * -1))
                    .ToList();

                var middleBand = Math.Sqrt(squaredValues.Sum() / periods);

                return new BollingerBands()
                {
                    LowerBand = sma20 - (2 * middleBand),
                    MiddleBand = sma20,
                    UpperBand = sma20 + (2 * middleBand)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"GetBollingerBands throw error: {ex.Message}");
                _logger.LogDebug($"GetBollingerBands throw error: {ex.StackTrace}");
                throw;
            }
        }
    }
}
