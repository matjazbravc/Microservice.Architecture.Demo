using Newtonsoft.Json;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Exchange.Rates.Contracts.Models;

[Serializable]
[ExcludeFromCodeCoverage]
[JsonObject(IsReference = false)]
public class CoinCapData
{
  [JsonProperty("id")]
  public string Id { get; set; }

  [JsonProperty("rank")]
  public string Rank { get; set; }

  [JsonProperty("symbol")]
  public string Symbol { get; set; }

  [JsonProperty("name")]
  public string Name { get; set; }

  [JsonProperty("supply")]
  public string Supply { get; set; }

  [JsonProperty("maxSupply")]
  public string MaxSupply { get; set; }

  [JsonProperty("marketCapUsd")]
  public string MarketCapUsd { get; set; }

  [JsonProperty("volumeUsd24Hr")]
  public string VolumeUsd24Hr { get; set; }

  [JsonProperty("priceUsd")]
  public string PriceUsd { get; set; }

  [JsonProperty("changePercent24Hr")]
  public string ChangePercent24Hr { get; set; }

  [JsonProperty("vwap24Hr")]
  public string Vwap24Hr { get; set; }
}
