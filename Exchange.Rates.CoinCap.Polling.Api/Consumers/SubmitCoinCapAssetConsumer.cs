using Exchange.Rates.CoinCap.Polling.Api.Services;
using Exchange.Rates.Contracts.Messages;
using MassTransit;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Exchange.Rates.CoinCap.Polling.Api.Consumers
{
    public sealed class SubmitCoinCapAssetConsumer : IConsumer<SubmitCoinCapAssetId>
    {
        private readonly ILogger<SubmitCoinCapAssetConsumer> _logger;
        private readonly ICoinCapAssetsApi _coinCapAssetsApi;

        public SubmitCoinCapAssetConsumer(ILogger<SubmitCoinCapAssetConsumer> logger,
            ICoinCapAssetsApi coinCapAssetsApi)
        {
            _logger = logger;
            _coinCapAssetsApi = coinCapAssetsApi;
        }

        public async Task Consume(ConsumeContext<SubmitCoinCapAssetId> context)
        {
	        _logger.LogDebug(nameof(Consume));
			if (context.RequestId != null && !string.IsNullOrWhiteSpace(context.Message.Id))
            {
	            var result = await _coinCapAssetsApi.GetAssetData(context.Message.Id).ConfigureAwait(false);
	            if (result.Data == null)
	            {
		            await context.RespondAsync<CoinCapAssetRejected>(new
		            {
			            context.Message.EventId,
			            InVar.Timestamp,
			            context.Message.Id,
			            Reason = $"Asset Data for a {context.Message.Id} is not available"
		            });
	            }
	            else
	            {
		            await context.RespondAsync<CoinCapAssetAccepted>(new
		            {
			            context.Message.EventId,
			            InVar.Timestamp,
			            context.Message.Id,
			            AssetData = result,
			            Message = "Asset Data"
		            });
	            }
            }
        }
    }
}
