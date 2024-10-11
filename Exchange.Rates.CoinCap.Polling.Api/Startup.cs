using Exchange.Rates.CoinCap.Polling.Api.Options;
using Exchange.Rates.CoinCap.Polling.Api.Policies;
using Exchange.Rates.CoinCap.Polling.Api.Services;
using Exchange.Rates.Contracts.Messages;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using System;
using System.Net.Http;
using System.Reflection;

namespace Exchange.Rates.CoinCap.Polling.Api;

public class Startup(IConfiguration configuration)
{
  private const string SERVICE_NAME = "Exchange.Rates.CoinCap.Polling.Api";

  public IConfiguration Configuration { get; } = configuration;

  public void ConfigureServices(IServiceCollection services)
  {
    // Add services required for using options
    services.AddOptions();

    // Configure CoinCapAssetsApiOptions
    var exchangeratesApiOptions = Configuration.GetSection(nameof(CoinCapAssetsApiOptions));
    services.Configure<CoinCapAssetsApiOptions>(options =>
    {
      options.HandlerLifetimeMinutes = Convert.ToInt32(exchangeratesApiOptions[nameof(CoinCapAssetsApiOptions.HandlerLifetimeMinutes)]);
      options.Url = exchangeratesApiOptions[nameof(CoinCapAssetsApiOptions.Url)];
    });

    // Configure MassTransitOptions
    var massTransitOptions = Configuration.GetSection(nameof(MassTransitOptions));
    services.Configure<MassTransitOptions>(options =>
    {
      options.Host = massTransitOptions[nameof(MassTransitOptions.Host)];
      options.Username = massTransitOptions[nameof(MassTransitOptions.Username)];
      options.Password = massTransitOptions[nameof(MassTransitOptions.Password)];
      options.QueueName = massTransitOptions[nameof(MassTransitOptions.QueueName)];
      options.ReceiveEndpointPrefetchCount = Convert.ToInt32(massTransitOptions[nameof(MassTransitOptions.ReceiveEndpointPrefetchCount)]);
    });

    // Configure DI for application services
    RegisterServices(services);

    var handlerLifetimeMinutes = Convert.ToInt32(exchangeratesApiOptions[nameof(CoinCapAssetsApiOptions.HandlerLifetimeMinutes)]);
    services.AddHttpClient<ICoinCapAssetsApi, CoinCapAssetsApi>()
        .AddPolicyHandler(RetryPolicies.GetRetryPolicy())
        .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler()
        {
          ClientCertificateOptions = ClientCertificateOption.Manual,
          ServerCertificateCustomValidationCallback = (_, _, _, _) => true
        })
        .SetHandlerLifetime(TimeSpan.FromMinutes(handlerLifetimeMinutes));

    // Formats the endpoint names usink kebab-case (dashed snake case)
    services.TryAddSingleton(KebabCaseEndpointNameFormatter.Instance);

    // Add MassTransit support
    services.AddMassTransit(x =>
    {
      // Automatically discover Consumers
      x.AddConsumers(Assembly.GetExecutingAssembly());
      x.UsingRabbitMq((busRegContext, rabbitBusConfig) =>
      {
        rabbitBusConfig.Host(new Uri(massTransitOptions[nameof(MassTransitOptions.Host)]), rabbitHostConfig =>
        {
          rabbitHostConfig.Username(massTransitOptions[nameof(MassTransitOptions.Username)]);
          rabbitHostConfig.Password(massTransitOptions[nameof(MassTransitOptions.Password)]);
        });
        rabbitBusConfig.ReceiveEndpoint(massTransitOptions[nameof(MassTransitOptions.QueueName)], ecfg =>
        {
          ecfg.PrefetchCount = Convert.ToInt16(massTransitOptions[nameof(MassTransitOptions.ReceiveEndpointPrefetchCount)]);
          ecfg.ConfigureConsumers(busRegContext);
          ecfg.UseMessageRetry(r => r.Interval(5, 1000));
          ecfg.UseJsonSerializer();
        });
      });
    });

    services.AddCors();
    services.AddRouting(options => options.LowercaseUrls = true);
  }

  public static void Configure(IApplicationBuilder app, IWebHostEnvironment env)
  {
    if (env.IsDevelopment())
    {
      app.UseDeveloperExceptionPage();
    }

    app.UseCors(policy =>
    {
      policy.AllowAnyOrigin();
      policy.AllowAnyHeader();
      policy.AllowAnyMethod();
    });

    app.UseRouting();

    app.UseEndpoints(configure =>
    {
      configure.MapGet("/", async context =>
      {
        await context.Response.WriteAsync(SERVICE_NAME);
      });
    });
  }

  protected virtual void RegisterServices(IServiceCollection services)
  {
    services.AddScoped<ICoinCapAssetsApi, CoinCapAssetsApi>();
  }
}