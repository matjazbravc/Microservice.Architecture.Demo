using Exchange.Rates.Tests.Services;
using Exchange.Rates.Tests.Services.Factories;
using Exchange.Rates.Tests.Services.Fixtures;
using System.Threading.Tasks;
using Xunit;

namespace Exchange.Rates.Tests.IntegrationTests;

public class ExchangeRatesCoinCapControllerTests : ControllerCoinCapTestsBase
{
  private const string BASE_URL = "/api/exchangeratescoincap/";
  private readonly HttpClientHelper _httpClientHelper;

  public ExchangeRatesCoinCapControllerTests(WebApiCoinCapTestFactory factory)
      : base(factory)
  {
    _httpClientHelper = new HttpClientHelper(Client);
  }

  [Fact]
  public async Task AssetInfoTest()
  {
    const string ID = "bitcoin";
    var result = await _httpClientHelper.GetAsync<string>(BASE_URL + $"assetinfo?id={ID}");
    Assert.NotNull(result);
  }
}
