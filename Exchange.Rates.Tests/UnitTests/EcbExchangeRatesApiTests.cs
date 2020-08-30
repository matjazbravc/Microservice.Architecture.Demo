using Exchange.Rates.Ecb.Polling.Api;
using Exchange.Rates.Ecb.Polling.Api.Services;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Xunit;

namespace Exchange.Rates.Tests.UnitTests;

public class EcbExchangeRatesApiTests(
  WebApplicationFactory<Startup> factory)
  : IClassFixture<WebApplicationFactory<Startup>>
{
  [Fact]
  public async Task GetLatestRatesTest_POSITIVE()
  {
    // Arrange
    var service = factory.Services.GetRequiredService<IEcbExchangeRatesApi>();
    // Act
    var result = await service.GetLatestRates();
    // Assert
    Assert.NotNull(result.Rates);
  }

  [Fact]
  public async Task GetLatestRatesTest_NEGATIVE()
  {
    // Arrange
    var service = factory.Services.GetRequiredService<IEcbExchangeRatesApi>();
    // Act
    var result = await service.GetLatestRates("XYZ");
    // Assert
    Assert.Null(result.Rates);
  }
}
