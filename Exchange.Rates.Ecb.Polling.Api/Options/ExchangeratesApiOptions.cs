using System.Diagnostics.CodeAnalysis;

namespace Exchange.Rates.Ecb.Polling.Api.Options
{
    [ExcludeFromCodeCoverage]
    public class ExchangeratesApiOptions
    {
        public int HandlerLifetimeMinutes { get; set; } = 5;

        public string Name { get; set; }

        public string Url { get; set; }
        
		public string AccessKey { get; set; }
    }
}
