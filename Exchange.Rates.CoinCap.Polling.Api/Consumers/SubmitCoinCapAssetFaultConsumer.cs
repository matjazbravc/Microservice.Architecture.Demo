using Exchange.Rates.Contracts.Messages;
using MassTransit;
using System;
using System.Threading.Tasks;

namespace Exchange.Rates.CoinCap.Polling.Api.Consumers
{
    public class SubmitCoinCapAssetFaultConsumer : IConsumer<Fault<SubmitCoinCapAssetId>>
    {
        public Task Consume(ConsumeContext<Fault<SubmitCoinCapAssetId>> context)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine();
            Console.WriteLine("There was an error with requesting a SubmitCoinCapAssetId");
            Console.ResetColor();
            return Task.CompletedTask;
        }
    }
}
