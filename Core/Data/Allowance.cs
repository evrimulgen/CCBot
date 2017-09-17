using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Core.Data
{
    public class Allowance
    {
        [JsonProperty("cost")]
        public double Cost { get; set; }
        [JsonProperty("remaining")]
        public double Remaining { get; set; }
    }
}
