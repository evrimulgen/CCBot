using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Data
{
    public class MarketTriple
    {
        public MarketTriple(string bittrexLiteral, string cryptoLiteral, string exchangeName)
        {
            BittrexLiteral = bittrexLiteral;
            CryptoWatchLiteral = cryptoLiteral;
            ExchangeName = exchangeName;
        }

        public string BittrexLiteral { get; set; }
        public string CryptoWatchLiteral { get; set; }
        public string ExchangeName { get; set; }
    }
}
