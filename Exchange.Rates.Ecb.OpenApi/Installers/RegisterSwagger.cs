using Exchange.Rates.Ecb.OpenApi.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Exchange.Rates.Ecb.OpenApi.Installers;

internal class RegisterSwagger : IServiceRegistration
{
  public void Register(IServiceCollection services, IConfiguration config)
  {
    services.AddSwaggerGen(options =>
    {
      options.SwaggerDoc("v1", new OpenApiInfo
      {
        Title = "Exchange.Rates.Ecb.OpenApi",
        Description = "ECB Foreign exchange rates published by the European Central Bank"
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
