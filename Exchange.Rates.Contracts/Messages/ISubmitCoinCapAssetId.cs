using Exchange.Rates.Contracts.Messages.Base;

namespace Exchange.Rates.Contracts.Messages;

public interface ISubmitCoinCapAssetId : IBaseContract
{
  string Id { get; }
}
