using Exchange.Rates.CoinCap.OpenApi;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Exchange.Rates.Tests.Services.Startups
{
    public class TestCoinCapStartup : Startup
    {
        public TestCoinCapStartup(IConfiguration configuration) 
            : base(configuration)
        {
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            // Build new configuration from test settings file
            var testConfiguration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.coincap.json")
                .Build();
            // Override base configuration
            Configuration = testConfiguration;
            base.ConfigureServices(services);
        }
    }
}
