using Exchange.Rates.CoinCap.Polling.Api.Services;
using Exchange.Rates.CoinCap.Polling.Api;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Xunit;

namespace Exchange.Rates.Tests.UnitTests;

public class CoinCapAssetsApiTests(
  WebApplicationFactory<Startup> factory)
  : IClassFixture<WebApplicationFactory<Startup>>
{
  [Fact]
  public async Task GetAssetDataTest_POSITIVE()
  {
    // Arrange
    var service = factory.Services.GetRequiredService<ICoinCapAssetsApi>();
    // Act
    var result = await service.GetAssetData("bitcoin");
    // Assert
    Assert.NotNull(result.Data);
  }

  [Fact]
  public async Task GetAssetDataTest_NEGATIVE()
  {
    // Arrange
    var service = factory.Services.GetRequiredService<ICoinCapAssetsApi>();
    // Act
    var result = await service.GetAssetData("xyz");
    // Assert
    Assert.Null(result.Data);
  }
}
