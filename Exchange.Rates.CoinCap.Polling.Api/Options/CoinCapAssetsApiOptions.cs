using System.Diagnostics.CodeAnalysis;

namespace Exchange.Rates.CoinCap.Polling.Api.Options
{
    [ExcludeFromCodeCoverage]
    public class CoinCapAssetsApiOptions
    {
        public int HandlerLifetimeMinutes { get; set; } = 5;

        public string Name { get; set; }

		public string Url { get; set; }
    }
}
