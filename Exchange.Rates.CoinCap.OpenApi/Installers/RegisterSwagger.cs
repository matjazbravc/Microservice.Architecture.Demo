using Exchange.Rates.CoinCap.OpenApi.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Exchange.Rates.CoinCap.OpenApi.Installers;

internal class RegisterSwagger : IServiceRegistration
{
  public void Register(IServiceCollection services, IConfiguration config)
  {
    services.AddSwaggerGen(options =>
    {
      options.SwaggerDoc("v1", new OpenApiInfo
      {
        Title = "Exchange.Rates.CoinCap.OpenApi",
        Description = "API for real-time pricing and market activity for over 1,000 cryptocurrencies"
      });
      var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
      var xmlDocFile = Path.Combine(AppContext.BaseDirectory, xmlFile);
      if (!File.Exists(xmlDocFile))
      {
        return;
      }
      options.IncludeXmlComments(xmlDocFile);
      options.DescribeAllParametersInCamelCase();
      options.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
    });
  }
}
