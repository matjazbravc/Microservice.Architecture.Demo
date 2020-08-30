using Exchange.Rates.Contracts.Messages;
using MassTransit;
using System;
using System.Threading.Tasks;

namespace Exchange.Rates.Ecb.Polling.Api.Consumers
{
    public class SubmitExchangeRateSymbolsFaultConsumer : IConsumer<Fault<SubmitEcbExchangeRateSymbols>>
    {
        public Task Consume(ConsumeContext<Fault<SubmitEcbExchangeRateSymbols>> context)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine();
            Console.WriteLine($"There was an error with requesting a SubmitExchangeRateSymbols");
            Console.ResetColor();
            return Task.CompletedTask;
        }
    }
}
