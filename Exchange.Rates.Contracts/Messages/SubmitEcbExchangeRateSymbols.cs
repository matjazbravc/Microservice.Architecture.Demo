using Exchange.Rates.Contracts.Messages.Base;
using System.Collections.Generic;

namespace Exchange.Rates.Contracts.Messages
{
    public interface SubmitEcbExchangeRateSymbols : IBaseContract
    {
        List<string> Symbols { get; }
    }
}
