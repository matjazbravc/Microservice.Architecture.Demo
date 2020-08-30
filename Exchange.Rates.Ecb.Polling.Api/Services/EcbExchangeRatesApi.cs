using Exchange.Rates.Contracts.Models;
using Exchange.Rates.Core.Extensions;
using Exchange.Rates.Ecb.Polling.Api.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System;
using System.Net.Http.Headers;

namespace Exchange.Rates.Ecb.Polling.Api.Services
{
    public class EcbExchangeRatesApi : IEcbExchangeRatesApi
    {
        private readonly ILogger<EcbExchangeRatesApi> _logger;
        private readonly ExchangeratesApiOptions _options;
        private readonly HttpClient _httpClient;
        private readonly JsonSerializer _serializer;

        public EcbExchangeRatesApi(ILogger<EcbExchangeRatesApi> logger, IOptions<ExchangeratesApiOptions> options, HttpClient httpClient)
        {
            _logger = logger;
            _options = options.Value;
            _httpClient = httpClient;
			_httpClient.Timeout = TimeSpan.FromSeconds(15);
			_httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _serializer = new JsonSerializer();
        }

		public async Task<EcbCurrencyExchange> GetLatestRates(string symbols)
        {
            var result = new EcbCurrencyExchange();
            try
            {
                var uri = new Uri(_options.Url).Append($"/latest?access_key={_options.AccessKey}&base=EUR&symbols={symbols}").AbsoluteUri;
                var isValid = Uri.IsWellFormedUriString(uri, UriKind.Absolute);
                if (!isValid)
                {
                    _logger.LogCritical($"Uri {uri} is invalid!");
                    return result;
                }
                var response = await _httpClient.GetAsync(uri).ConfigureAwait(false);
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogCritical($"Exchange Rates API Failed with HTTP Status Code {response.StatusCode} at: {DateTimeOffset.Now}");
                    return result;
                }

                using var sr = new StreamReader(await response.Content.ReadAsStreamAsync());
                using var jsonTextReader = new JsonTextReader(sr);
                result = _serializer.Deserialize<EcbCurrencyExchange>(jsonTextReader);
                if (result is {Rates: null})
                {
	                _logger.LogCritical("Exchange rates not returned from API");
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
}