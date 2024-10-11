using Exchange.Rates.Contracts.Messages.Base;

namespace Exchange.Rates.Contracts.Messages;

public interface IEcbExchangeRatesRejected : IBaseContract
{
  string Symbols { get; }

  string Reason { get; }
}
