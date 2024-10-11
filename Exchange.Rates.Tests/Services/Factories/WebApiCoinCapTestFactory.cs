using Exchange.Rates.CoinCap.OpenApi;
using Exchange.Rates.Tests.Services.Startups;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using System.IO;

namespace Exchange.Rates.Tests.Services.Factories;

/// <summary>
/// Customized WebApplicationFactory
/// </summary>
public class WebApiCoinCapTestFactory : WebApplicationFactory<TestCoinCapStartup>
{
  public TService GetRequiredService<TService>()
  {
    if (Server == null)
    {
      CreateDefaultClient();
    }
    return Server.Host.Services.GetRequiredService<TService>();
  }

  protected override void ConfigureWebHost(IWebHostBuilder builder)
  {
    builder
      .UseTestServer()
      .UseEnvironment("Test")
      .UseContentRoot(".")
      .ConfigureTestServices(services =>
      {
        services.BuildServiceProvider();
      });

    // Call base Configuration
    base.ConfigureWebHost(builder);
  }

  protected override IWebHostBuilder CreateWebHostBuilder()
  {
    IWebHostBuilder hostBuilder = new WebHostBuilder()
      .UseContentRoot(Directory.GetCurrentDirectory())
      .ConfigureAppConfiguration((context, _) =>
      {
        context.HostingEnvironment.ApplicationName = typeof(Program).Assembly.GetName().Name;
      })
      .UseStartup<TestCoinCapStartup>();
    return hostBuilder;
  }
}