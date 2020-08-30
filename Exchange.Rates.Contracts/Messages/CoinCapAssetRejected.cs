using Exchange.Rates.Contracts.Messages.Base;

namespace Exchange.Rates.Contracts.Messages
{
    public interface CoinCapAssetRejected : IBaseContract
    {
        string Id { get; }

        string Reason { get; }
    }
}
