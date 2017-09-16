using System.Collections.Generic;
using Newtonsoft.Json;

namespace Core.Data
{
    public interface ICurrencies
    {
        
    }

    public class Currencies : ICurrencies
    {
        [JsonProperty("success")]
        public bool Success {get; set; }
        [JsonProperty("message")]
        public string Message { get; set; }
        [JsonProperty("result")]
        public IEnumerable<SingleCurrency> Result { get; set; }

        public IEnumerable<SingleCurrency> GetApiResult()
        {
            return Result;
        }

        public void SetResult(IEnumerable<SingleCurrency> list)
        {
            Result = list;
        }
    }
    public class SingleCurrency
    {
        public string CurrencyDto { get; set; }
        public string CurrencyLong { get; set; }
        public int MinConfirmation { get; set; }
        public double TxFee { get; set; }
        public bool IsActive { get; set; }
        public string CoinType { get; set; }
        public string BaseAddress { get; set; }
    }
}