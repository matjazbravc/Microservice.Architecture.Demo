using Exchange.Rates.Contracts.Messages.Base;

namespace Exchange.Rates.Contracts.Messages
{
    public interface EcbExchangeRatesRejected : IBaseContract
    {
        string Symbols { get; }

        string Reason { get; }
    }
}
