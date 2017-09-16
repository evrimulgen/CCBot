using Newtonsoft.Json;

namespace Core.Data
{
    public class Ticker
    {
        [JsonProperty("message")]
        public string Message { get; set; }
        [JsonProperty("success")]
        public bool Success { get; set; }
        [JsonProperty("result")]
        public BidAskLastTriple Result { get; set; }
        public string MarketLiteral { get; set; }  
    }

    public class BidAskLastTriple
    {
        [JsonProperty("Bid")]
        public decimal Bid { get; set; }
        [JsonProperty("Ask")]
        public decimal Ask { get; set; }
        [JsonProperty("Last")]
        public decimal Last { get; set; }
    }
}
