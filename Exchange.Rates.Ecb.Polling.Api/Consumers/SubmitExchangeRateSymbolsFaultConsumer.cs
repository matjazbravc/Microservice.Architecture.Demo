using Exchange.Rates.Contracts.Messages;
using MassTransit;
using System;
using System.Threading.Tasks;

namespace Exchange.Rates.Ecb.Polling.Api.Consumers;

public class SubmitExchangeRateSymbolsFaultConsumer : IConsumer<Fault<ISubmitEcbExchangeRateSymbols>>
{
  public Task Consume(ConsumeContext<Fault<ISubmitEcbExchangeRateSymbols>> context)
  {
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine();
    Console.WriteLine($"There was an error with requesting a SubmitExchangeRateSymbols");
    Console.ResetColor();
    return Task.CompletedTask;
  }
}
