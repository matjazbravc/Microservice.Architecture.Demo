using Exchange.Rates.Contracts.Messages;
using Exchange.Rates.Ecb.Polling.Api.Services;
using MassTransit;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace Exchange.Rates.Ecb.Polling.Api.Consumers;

public sealed class SubmitExchangeRateSymbolsConsumer : IConsumer<ISubmitEcbExchangeRateSymbols>
{
  private readonly ILogger<SubmitExchangeRateSymbolsConsumer> _logger;
  private readonly IEcbExchangeRatesApi _ecbExchangeRatesApi;

  public SubmitExchangeRateSymbolsConsumer(ILogger<SubmitExchangeRateSymbolsConsumer> logger,
      IEcbExchangeRatesApi ecbExchangeRatesApi)
  {
    _logger = logger;
    _ecbExchangeRatesApi = ecbExchangeRatesApi;
  }

  public async Task Consume(ConsumeContext<ISubmitEcbExchangeRateSymbols> context)
  {
    if (context.RequestId != null)
    {
      if (context.Message.Symbols?.Any() == true)
      {
        var symbols = string.Join(",", context.Message.Symbols);
        var result = await _ecbExchangeRatesApi.GetLatestRates(symbols).ConfigureAwait(false);
        if (result.Rates == null)
        {
          await context.RespondAsync<IEcbExchangeRatesRejected>(new
          {
            context.Message.EventId,
            InVar.Timestamp,
            context.Message.Symbols,
            Reason = $"Exchange Rates for a {symbols} are not available"
          });
        }
        else
        {
          await context.RespondAsync<IEcbExchangeRatesAccepted>(new
          {
            context.Message.EventId,
            InVar.Timestamp,
            context.Message.Symbols,
            CurrencyExchange = result,
            Message = "Exchange Rates Symbols"
          });
        }
      }
    }
  }
}
