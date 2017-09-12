using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Core.Data
{
    public class OrderBook
    {
        [JsonProperty("success")]
        public bool Success { get; set; }
        [JsonProperty("message")]
        public string Message { get; set; }
        [JsonProperty("result")]
        public Orders Result { get; }
        public string MarketLiteral { get; set; }

        public OrderBook()
        {
            Result = new Orders()
            {
                BuyOrders = new List<Buy>(),
                SellOrders = new List<Sell>()
            };
        }
    }

    public class Orders
    {
        [JsonProperty("buy")]
        public IList<Buy> BuyOrders;
        [JsonProperty("sell")]
        public IList<Sell> SellOrders;
    }

    public class BuyOrders
    {
        public IEnumerable<Buy> Buys;
    }

    public class SellOrders
    {
        public IEnumerable<Sell> Sells;
    }

    public class Buy
    {
        [JsonProperty("Quantity")]
        public double Quantity { get; set; }
        [JsonProperty("Rate")]
        public double Rate { get; set; }
    }

    public class Sell
    {
        [JsonProperty("Quantity")]
        public double Quantity { get; set; }
        [JsonProperty("Rate")]
        public double Rate { get; set; }
    }
}
