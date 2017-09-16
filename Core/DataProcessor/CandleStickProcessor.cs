using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.APIs;
using Core.Extensions;
using Microsoft.Extensions.Logging;

namespace Core.DataProcessor
{
    public interface ICandleStickProcessor
    {
        Task<double> GetMarketChangePercentage(string bittrexMarketLiteral, int timeFrame, IList<int> candleTimeIntervals);
    }

    public class CandleStickProcessor : ICandleStickProcessor
    {
        private readonly ILogger<CandleStickProcessor> _logger;
        private readonly ICryptoWatchApi _cryptoWatchApi;

        public CandleStickProcessor(ILogger<CandleStickProcessor> logger, ICryptoWatchApi cryptoWatchApi)
        {
            _logger = logger;
            _cryptoWatchApi = cryptoWatchApi;
        }

        /// <summary>
        /// Get the percentual change of a market in the given time frame in seconds. This can be very expensive on nanoseconds CPU time, use with caution. 
        /// </summary>
        /// <param name="bittrexMarketLiteral">Bittrex marketliteral F.E: BTC-LTC</param>
        /// <param name="timeFrame">Timeframe from now, until seconds ago. F.E: 3600 gets the data from one hour ago until now</param>
        /// <param name="candleTimeIntervals">Seconds dating back from now (The provided timeframe)</param>
        /// <returns>Returns the percentual change of the market in the given time frame in seconds.</returns>
        public async Task<double> GetMarketChangePercentage(string bittrexMarketLiteral, int timeFrame, IList<int> candleTimeIntervals)
        {
            var startCandleDataFrom =
                DateTime.Now.DateTimeToUnixTimeStamp().SubstractsecondsFromUnixTimeStamp(timeFrame);

            var candleStickData = await _cryptoWatchApi.GetCandleData(bittrexMarketLiteral,
                OlhcBeforeAfterParam.After, startCandleDataFrom, candleTimeIntervals);

            return 0;
        }
    }
}
