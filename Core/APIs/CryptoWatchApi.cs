using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Core.Data;
using Core.Extensions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Core.APIs
{
    public enum OlhcBeforeAfterParam
    {
        Before,
        After
    }

    public interface ICryptoWatchApi
    {
        Task<CandleData> GetCandleData(string bittrexLiteral, OlhcBeforeAfterParam param, double unixTimeStamp, IEnumerable<int> timeIntervalsInSeconds);
    }

    public class CryptoWatchApi : ICryptoWatchApi
    {
        private readonly ILogger<CryptoWatchApi> _logger;
        private static readonly string ExchangeString = $"Kraken";

        public CryptoWatchApi(ILogger<CryptoWatchApi> logger)
        {
            _logger = logger;
        }

        public async Task<CandleData> GetCandleData(string bittrexLiteral, OlhcBeforeAfterParam param, double unixTimeStamp, IEnumerable<int> timeIntervalsInSeconds)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var transformedLiteral = bittrexLiteral.ConvertBittrexToCryptoWatchLiteral();
                    var intervalsInSeconds = timeIntervalsInSeconds as int[] ?? timeIntervalsInSeconds.ToArray();
                    var intervalParam = string.Join(",", intervalsInSeconds.Select(x => x.ToString()).ToArray());

                    var uri =
                        $"https://api.cryptowat.ch/markets/{ExchangeString}/{transformedLiteral}/ohlc?{param}={unixTimeStamp}&periods={intervalParam}".ToLower();

                    var json = await client.GetStringAsync(new Uri(uri));
                    var candleResult = JObject.Parse(json);
                    var candleData = new CandleData();

                    foreach (var interval in intervalsInSeconds)
                    {
                        var result = candleResult["result"][$"{interval}"]
                            .Select(cs => cs.ToArray()).Select(cs => new CandleStick()
                            {
                                CloseTime = cs[0].ToObject<decimal>(),
                                OpenPrice = cs[1].ToObject<decimal>(),
                                HighPrice = cs[2].ToObject<decimal>(),
                                LowPrice = cs[3].ToObject<decimal>(),
                                ClosePrice = cs[4].ToObject<decimal>(),
                                Volume = cs[5].ToObject<decimal>()
                            })
                            .ToList();

                        candleData.ResultSet.Add(interval, result);
                    }

                    _logger.LogInformation($"Finished fetching candledata for {bittrexLiteral} - Success: {candleData.ResultSet.First().Value.Count > 0}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"GetCandleData threw an exception!: {ex.Message}");
                _logger.LogDebug($"GetCandleData threw an exception!: {ex.StackTrace}");
            }

            return null;
        }
    }
}
