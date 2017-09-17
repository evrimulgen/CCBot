using System.Collections.Generic;
using Newtonsoft.Json;

namespace Core.Data
{
    public class CandleData
    {
        [JsonProperty("result")]
        public IDictionary<int, IList<CandleStick>> ResultSet { get; set; }
        [JsonProperty("allowance")]
        public Allowance Allowance { get; set; }

        public CandleData()
        {
            ResultSet = new Dictionary<int, IList<CandleStick>>();
        }
    }

    public class CandleStick
    {
        [JsonProperty("CloseTime")]
        public decimal CloseTime { get; set; }
        [JsonProperty("OpenPrice")]
        public decimal OpenPrice { get; set; }
        [JsonProperty("HighPrice")]
        public decimal HighPrice { get; set; }
        [JsonProperty("LowPrice")]
        public decimal LowPrice { get; set; }
        [JsonProperty("ClosePrice")]
        public decimal ClosePrice { get; set; }
        [JsonProperty("Volume")]
        public decimal Volume { get; set; }
        public decimal Average => (OpenPrice + ClosePrice) / 2;
    }
}