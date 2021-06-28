using System.Diagnostics.CodeAnalysis;

namespace Exchange.Rates.Ecb.OpenApi.Options
{
    [ExcludeFromCodeCoverage]
    public class MassTransitOptions
    {
        public string Host { get; set; }

        public string Username { get; set; }
        
        public string Password { get; set; }
        
        public string QueueName { get; set; }
        
        public int ReceiveEndpointPrefetchCount { get; set; } = 16;
    }
}
