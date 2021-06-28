using Exchange.Rates.CoinCap.Polling.Api.Services;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Xunit;

namespace Exchange.Rates.Tests.UnitTests
{
    public class CoinCapAssetsApiTests : IClassFixture<WebApplicationFactory<CoinCap.Polling.Api.Startup>>
    {
        private readonly WebApplicationFactory<CoinCap.Polling.Api.Startup> _factory;

        public CoinCapAssetsApiTests(WebApplicationFactory<CoinCap.Polling.Api.Startup> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task GetAssetDataTest_POSITIVE()
        {
            // Arrange
            var service = _factory.Services.GetRequiredService<ICoinCapAssetsApi>();
            // Act
            var result = await service.GetAssetData("bitcoin").ConfigureAwait(false);
            // Assert
            Assert.NotNull(result.Data);
        }

        [Fact]
        public async Task GetAssetDataTest_NEGATIVE()
        {
            // Arrange
            var service = _factory.Services.GetRequiredService<ICoinCapAssetsApi>();
            // Act
            var result = await service.GetAssetData("xyz").ConfigureAwait(false);
            // Assert
            Assert.Null(result.Data);
        }
    }
}
