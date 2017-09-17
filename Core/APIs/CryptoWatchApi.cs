using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Core.Data;
using Core.Extensions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
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
        Task<CandleData> GetCandleData(MarketTriple triple, OlhcBeforeAfterParam param, double unixTimeStamp, IEnumerable<int> timeIntervalsInSeconds);
        Task<CryptoWatchMarketPairData> GetPairs();
        Task<CryptoWatchMarketsData> GetMarkets();
    }

    public class CryptoWatchApi : ICryptoWatchApi
    {
        private readonly ILogger<CryptoWatchApi> _logger;

        public CryptoWatchApi(ILogger<CryptoWatchApi> logger)
        {
            _logger = logger;
        }

        public async Task<CandleData> GetCandleData(MarketTriple triple, OlhcBeforeAfterParam param, double unixTimeStamp, IEnumerable<int> timeIntervalsInSeconds)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var literal = triple.CryptoWatchLiteral;
                    var intervalsInSeconds = timeIntervalsInSeconds as int[] ?? timeIntervalsInSeconds.ToArray();
                    var intervalParam = string.Join(",", intervalsInSeconds.Select(x => x.ToString()).ToArray());

                    var uri =
                        $"https://api.cryptowat.ch/markets/{triple.ExchangeName}/{literal}/ohlc?{param}={unixTimeStamp}&periods={intervalParam}".ToLower();

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

                    _logger.LogInformation($"Finished fetching candledata for {literal} - Success: {candleData.ResultSet.First().Value.Count > 0}");
                    return candleData;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"GetCandleData threw an exception!: {ex.Message}");
                _logger.LogDebug($"GetCandleData threw an exception!: {ex.StackTrace}");
            }

            return null;
        }

        public async Task<CryptoWatchMarketPairData> GetPairs()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var uri =
                        $"https://api.cryptowat.ch/pairs";

                    var json = await client.GetStringAsync(new Uri(uri));
                    var marketPairs = JsonConvert.DeserializeObject<CryptoWatchMarketPairData>(json);

                    if (!marketPairs.ResultSet.IsNullOrEmpty())
                    {
                        return marketPairs;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"GetPairs threw an exception!: {ex.Message}");
                _logger.LogDebug($"GetPairs threw an exception!: {ex.StackTrace}");
            }

            return null;
        }

        public async Task<CryptoWatchMarketsData> GetMarkets()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var uri =
                        $"https://api.cryptowat.ch/markets";

                    var json = await client.GetStringAsync(new Uri(uri));
                    var marketsData = JsonConvert.DeserializeObject<CryptoWatchMarketsData>(json);

                    if (!marketsData.Result.IsNullOrEmpty())
                    {
                        return marketsData;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"GetMarkets threw an exception!: {ex.Message}");
                _logger.LogDebug($"GetMarkets threw an exception!: {ex.StackTrace}");
            }

            return null;
        }
    }
}
