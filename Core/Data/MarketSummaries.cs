using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Core.Data
{
    public class MarketSummaries
    {
        [JsonProperty("success")]
        public bool Success { get; set; }
        [JsonProperty("message")]
        public string Message { get; set; }
        [JsonProperty("result")]
        public IEnumerable<MarketSummary> Result { get; set; }
    }

    public class MarketSummary
    {
        [JsonProperty("MarketName")]
        public string MarketLiteral { get; set; }
        [JsonProperty("High")]
        public double High { get; set; }
        [JsonProperty("Low")]
        public double Low { get; set; }
        [JsonProperty("Volume")]
        public double Volume { get; set; }
        [JsonProperty("Last")]
        public double Last { get; set; }
        [JsonProperty("BaseVolume")]
        public double BaseVolume { get; set; }
        [JsonProperty("TimeStamp")]
        public DateTime TimeStamp { get; set; }
        [JsonProperty("Bid")]
        public double Bid { get; set; }
        [JsonProperty("Ask")]
        public double Ask { get; set; }
        [JsonProperty("OpenBuyOrders")]
        public int OpenBuyOrders { get; set; }
        [JsonProperty("OpenSellOrders")]
        public int OpenSellOrders { get; set; }
        [JsonProperty("PrevDay")]
        public double PrevDay { get; set; }
        [JsonProperty("Created")]
        public DateTime Created { get; set; }
    }

    public class MarketSummarySingle
    {
        [JsonProperty("success")]
        public bool Success { get; set; }
        [JsonProperty("message")]
        public string Message { get; set; }
        [JsonProperty("result")]
        public IEnumerable<MarketSummary> Result { get; set; }
    }
}
