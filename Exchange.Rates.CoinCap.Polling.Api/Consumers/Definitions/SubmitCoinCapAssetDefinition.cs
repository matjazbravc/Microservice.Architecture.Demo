using Exchange.Rates.CoinCap.Polling.Api.Options;
using MassTransit;
using Microsoft.Extensions.Options;

namespace Exchange.Rates.CoinCap.Polling.Api.Consumers.Definitions;

// Consumer definitions can be explicitely registered or discovered automatically using the AddConsumers
public class SubmitCoinCapAssetDefinition : ConsumerDefinition<SubmitCoinCapAssetConsumer>
{
  public SubmitCoinCapAssetDefinition(IOptions<MassTransitOptions> options)
  {
    // Override the default endpoint name, for whatever reason
    EndpointName = options.Value.QueueName;

    // Limit the number of messages consumed concurrently
    // this applies to the consumer only, not the endpoint
    ConcurrentMessageLimit = 16;
  }

  protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator,
      IConsumerConfigurator<SubmitCoinCapAssetConsumer> consumerConfigurator)
  {
    endpointConfigurator.UseMessageRetry(r => r.Interval(5, 1000));
    endpointConfigurator.UseInMemoryOutbox();
  }
}
