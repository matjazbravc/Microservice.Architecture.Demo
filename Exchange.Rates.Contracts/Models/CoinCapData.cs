using Newtonsoft.Json;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Exchange.Rates.Contracts.Models
{
    [Serializable]
    [ExcludeFromCodeCoverage]
    [JsonObject(IsReference = false)]
    public class CoinCapData
    {
        [JsonProperty("id")]
        public string id { get; set; }

        [JsonProperty("rank")]
        public string rank { get; set; }

        [JsonProperty("symbol")]
        public string symbol { get; set; }

        [JsonProperty("name")]
        public string name { get; set; }

        [JsonProperty("supply")]
        public string supply { get; set; }

        [JsonProperty("maxSupply")]
        public string maxSupply { get; set; }

        [JsonProperty("marketCapUsd")]
        public string marketCapUsd { get; set; }

        [JsonProperty("volumeUsd24Hr")]
        public string volumeUsd24Hr { get; set; }

        [JsonProperty("priceUsd")]
        public string priceUsd { get; set; }

        [JsonProperty("changePercent24Hr")]
        public string changePercent24Hr { get; set; }

        [JsonProperty("vwap24Hr")]
        public string vwap24Hr { get; set; }
    }

}
