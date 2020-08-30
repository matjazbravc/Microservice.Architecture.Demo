using Newtonsoft.Json;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Exchange.Rates.Contracts.Models
{
    /// <summary>
    /// Hold result of https://api.coincap.io/v2/assets/bitcoin request
    /// </summary>
    [Serializable]
    [ExcludeFromCodeCoverage]
    [JsonObject(IsReference = false)]
    public class CoinCapAsset
    {

        [JsonProperty("data")]
        public CoinCapData Data { get; set; }

        [JsonProperty("timestamp")]
        public long TimeStamp { get; set; }
    }
}
