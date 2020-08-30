using Exchange.Rates.Contracts.Models;
using System.Threading.Tasks;

namespace Exchange.Rates.Ecb.Polling.Api.Services;

public interface IEcbExchangeRatesApi
{
  Task<EcbCurrencyExchange> GetLatestRates(string symbols = "USD");
}