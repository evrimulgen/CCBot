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
    }
}
