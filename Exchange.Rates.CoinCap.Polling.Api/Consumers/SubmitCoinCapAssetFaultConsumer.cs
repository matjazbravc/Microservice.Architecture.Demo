using Exchange.Rates.Contracts.Messages;
using MassTransit;
using System;
using System.Threading.Tasks;

namespace Exchange.Rates.CoinCap.Polling.Api.Consumers;

public class SubmitCoinCapAssetFaultConsumer : IConsumer<Fault<ISubmitCoinCapAssetId>>
{
  public Task Consume(ConsumeContext<Fault<ISubmitCoinCapAssetId>> context)
  {
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine();
    Console.WriteLine("There was an error with requesting a ISubmitCoinCapAssetId");
    Console.ResetColor();
    return Task.CompletedTask;
  }
}
