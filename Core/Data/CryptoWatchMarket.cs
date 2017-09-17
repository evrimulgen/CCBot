using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Core.Data
{
    public class CryptoWatchMarketsData
    {
        [JsonProperty("result")]
        public IEnumerable<CryptoWatchMarket> Result{ get; set; }
        [JsonProperty("allowance")]
        public Allowance Allowance { get; set; }
    }

    public class CryptoWatchMarket
    {
        [JsonProperty("exchange")]
        public string Exchange { get; set; }
        [JsonProperty("pair")]
        public string Pair { get; set; }
        [JsonProperty("active")]
        public bool Active { get; set; }    
        [JsonProperty("route")]
        public string Route { get; set; }
    }
}