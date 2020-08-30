using System;

namespace Exchange.Rates.Contracts.Messages.Base
{
    public interface IBaseContract
    {
        Guid EventId { get; }

        DateTime Timestamp { get; }
    }
}
