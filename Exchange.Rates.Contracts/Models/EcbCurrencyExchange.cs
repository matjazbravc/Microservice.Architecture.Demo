using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Exchange.Rates.Contracts.Models
{
    [Serializable]
    [ExcludeFromCodeCoverage]
    [JsonObject(IsReference = false)]
    public class EcbCurrencyExchange
    {
        [JsonProperty("base")]
        public string Base { get; set; }

        [JsonProperty("date")]
        public DateTime Date { get; set; }

        [JsonProperty("rates")]
        public Dictionary<string, decimal> Rates { get; set; }
    }
}
