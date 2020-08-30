using Exchange.Rates.CoinCap.Polling.Api.Options;
using Exchange.Rates.Contracts.Models;
using Exchange.Rates.Core.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Exchange.Rates.CoinCap.Polling.Api.Services;

public class CoinCapAssetsApi : ICoinCapAssetsApi
{
  private readonly ILogger<CoinCapAssetsApi> _logger;
  private readonly CoinCapAssetsApiOptions _options;
  private readonly HttpClient _httpClient;
  private readonly JsonSerializer _serializer;

  public CoinCapAssetsApi(ILogger<CoinCapAssetsApi> logger, IOptions<CoinCapAssetsApiOptions> options, HttpClient httpClient)
  {
    _logger = logger;
    _options = options.Value;
    _httpClient = httpClient;
    _httpClient.Timeout = TimeSpan.FromSeconds(15);
    _httpClient.DefaultRequestHeaders.Accept.Clear();
    _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    _serializer = new JsonSerializer();
  }

  public async Task<CoinCapAsset> GetAssetData(string id)
  {
    var result = new CoinCapAsset();
    try
    {
      var uri = new Uri(_options.Url).Append(id).AbsoluteUri;
      var isValid = Uri.IsWellFormedUriString(uri, UriKind.Absolute);
      if (!isValid)
      {
        _logger.LogCritical($"Uri {uri} is invalid!");
        return result;
      }
      var response = await _httpClient.GetAsync(uri).ConfigureAwait(false);
      if (!response.IsSuccessStatusCode)
      {
        _logger.LogCritical($"CoinCap API Failed with HTTP Status Code {response.StatusCode} at: {DateTimeOffset.Now}");
        return result;
      }

      using var sr = new StreamReader(await response.Content.ReadAsStreamAsync());
      using var jsonTextReader = new JsonTextReader(sr);
      result = _serializer.Deserialize<CoinCapAsset>(jsonTextReader);
      if (result is { Data: null })
      {
        _logger.LogCritical("Assets not returned from API");
        return result;
      }
    }
    catch (Exception ex)
    {
      _logger.LogCritical(ex.Message);
    }
    return result;
  }
}