using Exchange.Rates.Contracts.Messages.Base;

namespace Exchange.Rates.Contracts.Messages;

public interface ICoinCapAssetRejected : IBaseContract
{
  string Id { get; }

  string Reason { get; }
}
