using Newtonsoft.Json;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System;

namespace Exchange.Rates.Tests.Services
{
	/// <summary>
	/// HttpClient helper class
	/// </summary>
	public class HttpClientHelper
	{
		public HttpClientHelper(HttpClient httpClient)
		{
			HttpClient = httpClient;
		}

		public HttpClient HttpClient { get; }

		public async Task<T> DeleteAsync<T>(string path)
		{
			using var response = await HttpClient.DeleteAsync(path).ConfigureAwait(false);
			return await GetContentAsync<T>(response);
		}

		public async Task<HttpStatusCode> DeleteAsync(string path)
		{
			var response = await HttpClient.DeleteAsync(path).ConfigureAwait(false);
			return response.StatusCode;
		}

		public async Task<T> GetAsync<T>(string path)
		{
			using var response = await HttpClient.GetAsync(path).ConfigureAwait(false);
			return await GetContentAsync<T>(response);
		}

		public async Task<T> PostAsync<T>(string path, T content)
		{
			return await PostAsync<T, T>(path, content);
		}

		public async Task<TOut> PostAsync<TIn, TOut>(string path, TIn content)
		{
			var json = content == null ?
				null :
				new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json");
			using var response = await HttpClient.PostAsync(path, json).ConfigureAwait(false);
			return await GetContentAsync<TOut>(response);
		}

		public async Task<TOut> PutAsync<TIn, TOut>(string path, TIn content)
		{
			var json = content == null ?
				null :
				new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json");
			using var response = await HttpClient.PutAsync(path, json).ConfigureAwait(false);
			return await GetContentAsync<TOut>(response);
		}

		private async Task<T> GetContentAsync<T>(HttpResponseMessage response)
		{
			response.EnsureSuccessStatusCode();
			var responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
			if (typeof(T) == typeof(string))
			{
				return (T)Convert.ChangeType(responseString, typeof(T));
			}
			return JsonConvert.DeserializeObject<T>(responseString);
		}
	}
}
