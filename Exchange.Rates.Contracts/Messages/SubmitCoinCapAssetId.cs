using Exchange.Rates.Contracts.Messages.Base;

namespace Exchange.Rates.Contracts.Messages
{
    public interface SubmitCoinCapAssetId : IBaseContract
    {
        string Id { get; }
    }
}
