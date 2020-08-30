using Exchange.Rates.Tests.Services.Factories;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Xunit;

namespace Exchange.Rates.Tests.Services.Fixtures;

/// <summary>
/// Base Controller tests IClassFixture
/// </summary>
public class ControllerEcbTestsBase : IClassFixture<WebApiEcbTestFactory>
{
  protected HttpClient Client;

  public ControllerEcbTestsBase(WebApiEcbTestFactory factory)
  {
    Client = factory.CreateClient();
    Client.Timeout = TimeSpan.FromSeconds(15);
    Client.DefaultRequestHeaders.Accept.Clear();
    Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
  }
}