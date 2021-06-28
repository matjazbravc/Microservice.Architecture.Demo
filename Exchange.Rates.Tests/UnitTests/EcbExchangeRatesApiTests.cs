using Exchange.Rates.Ecb.Polling.Api.Services;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Xunit;

namespace Exchange.Rates.Tests.UnitTests
{
    public class EcbExchangeRatesApiTests : IClassFixture<WebApplicationFactory<Exchange.Rates.Ecb.Polling.Api.Startup>>
    {
        private readonly WebApplicationFactory<Ecb.Polling.Api.Startup> _factory;

        public EcbExchangeRatesApiTests(WebApplicationFactory<Ecb.Polling.Api.Startup> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task GetLatestRatesTest_POSITIVE()
        {
            // Arrange
            var service = _factory.Services.GetRequiredService<IEcbExchangeRatesApi>();
            // Act
            var result = await service.GetLatestRates().ConfigureAwait(false);
            // Assert
            Assert.NotNull(result.Rates);
        }

        [Fact]
        public async Task GetLatestRatesTest_NEGATIVE()
        {
            // Arrange
            var service = _factory.Services.GetRequiredService<IEcbExchangeRatesApi>();
            // Act
            var result = await service.GetLatestRates("XYZ").ConfigureAwait(false);
            // Assert
            Assert.Null(result.Rates);
        }
    }
}
