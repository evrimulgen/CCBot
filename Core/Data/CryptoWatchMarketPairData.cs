using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Core.Data
{
    public class CryptoWatchMarketPairData
    {
        [JsonProperty("result")]
        public IList<MarketPair> ResultSet { get; set; }
        [JsonProperty("allowance")]
        public Allowance Allowance { get; set; }
    }

    public class MarketPair
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("base")]
        public MarketPairBase Base { get; set; }
        [JsonProperty("quote")]
        public MarketQuote Quote { get; set; }
        [JsonProperty("route")]
        public Uri Route { get; set; }

    }

    public class MarketPairBase
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("fiat")]
        public bool Fiat { get; set; }
        [JsonProperty("route")]
        public Uri Route { get; set; }
    }

    public class MarketQuote
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("name")]
        public string  Name { get; set; }
        [JsonProperty("fiat")]
        public bool Fiat { get; set; }
        [JsonProperty("route")]
        public Uri Route { get; set; }
    }
}

//{
//"id": "ltckrw",

//"base": {
//"id": "ltc",
//"name": "Litecoin",
//"fiat": false,
//"route": "https://api.cryptowat.ch/assets/ltc"
//},

//"quote": {
//"id": "krw",
//"name": "South Korean Won",
//"fiat": true,
//"route": "https://api.cryptowat.ch/assets/krw"
//},
//"route": "https://api.cryptowat.ch/pairs/ltckrw"
//}
