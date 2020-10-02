using Exchange.Rates.CoinCap.Polling.Api.Options;
using Exchange.Rates.CoinCap.Polling.Api.Policies;
using Exchange.Rates.CoinCap.Polling.Api.Services;
using GreenPipes;
using HealthChecks.UI.Client;
using MassTransit.Definition;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Reflection;
using System;

namespace Exchange.Rates.CoinCap.Polling.Api
{
    public class Startup
    {
        private const string SERVICE_NAME = "Exchange.Rates.CoinCap.Polling.Api";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

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
                options.ReceiveEndpointName = massTransitOptions[nameof(MassTransitOptions.ReceiveEndpointName)];
                options.ReceiveEndpointPrefetchCount = Convert.ToInt32(massTransitOptions[nameof(MassTransitOptions.ReceiveEndpointPrefetchCount)]);
            });

            // Configure HealthChecksOptions
            var healthChecksOptions = Configuration.GetSection(nameof(HealthChecksOptions));
            services.Configure<HealthChecksOptions>(options =>
            {
                options.PrivateMemoryName = healthChecksOptions[nameof(HealthChecksOptions.PrivateMemoryName)];
                options.PrivateMemoryMax = Convert.ToInt32(healthChecksOptions[nameof(HealthChecksOptions.PrivateMemoryMax)]);
                options.ProcessAllocatedMemoryName = healthChecksOptions[nameof(HealthChecksOptions.ProcessAllocatedMemoryName)];
                options.ProcessAllocatedMemoryMax = Convert.ToInt32(healthChecksOptions[nameof(HealthChecksOptions.ProcessAllocatedMemoryMax)]);
            });

            // Configure DI for application services
            RegisterServices(services);

            // https://asp.net-hacker.rocks/2020/08/20/health-checks.html
            services.AddHealthChecks()
                .AddPrivateMemoryHealthCheck(Convert.ToInt32(healthChecksOptions[nameof(HealthChecksOptions.PrivateMemoryMax)]), healthChecksOptions[nameof(HealthChecksOptions.PrivateMemoryName)])
                .AddProcessAllocatedMemoryHealthCheck(Convert.ToInt32(healthChecksOptions[nameof(HealthChecksOptions.ProcessAllocatedMemoryMax)]), healthChecksOptions[nameof(HealthChecksOptions.ProcessAllocatedMemoryName)])
                .AddUrlGroup(sp =>
                {
                    var options = sp.GetRequiredService<IOptions<CoinCapAssetsApiOptions>>().Value;
                    return new Uri(options.Url);
                },
                name: exchangeratesApiOptions[nameof(CoinCapAssetsApiOptions.Name)],
                failureStatus: HealthStatus.Degraded);

            // https://localhost:{port}/healthchecks-ui
            services.AddHealthChecksUI(opt =>
            {
                opt.SetEvaluationTimeInSeconds(15); // time in seconds between check
                opt.MaximumHistoryEntriesPerEndpoint(60); // maximum history of checks
                opt.SetApiMaxActiveRequests(1); // api requests concurrency
            })
            .AddInMemoryStorage();

            var handlerLifetimeMinutes = Convert.ToInt32(exchangeratesApiOptions[nameof(CoinCapAssetsApiOptions.HandlerLifetimeMinutes)]);
            services.AddHttpClient<ICoinCapAssetsApi, CoinCapAssetsApi>()
                .AddPolicyHandler(RetryPolicies.GetRetryPolicy())
                .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler()
                {
                    ClientCertificateOptions = ClientCertificateOption.Manual,
                    ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, cetChain, policyErrors) =>
                    {
                        return true;
                    }
                })
                .SetHandlerLifetime(TimeSpan.FromMinutes(handlerLifetimeMinutes));

            // Formats the endpoint names usink kebab-case (dashed snake case)
            services.TryAddSingleton(KebabCaseEndpointNameFormatter.Instance);

            // Add MassTransit support
            services.AddMassTransit(config =>
            {
                // Automatically discover Consumers
                config.AddConsumers(Assembly.GetExecutingAssembly());

                // Add Service Bus
                config.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(cfg =>
                {
                    cfg.UseHealthCheck(provider);
                    cfg.Host(new Uri(massTransitOptions[nameof(MassTransitOptions.Host)]), hcfg =>
                    {
                        hcfg.Username(massTransitOptions[nameof(MassTransitOptions.Username)]);
                        hcfg.Password(massTransitOptions[nameof(MassTransitOptions.Password)]);
                    });
                    cfg.ReceiveEndpoint(massTransitOptions[nameof(MassTransitOptions.ReceiveEndpointName)], ecfg =>
                    {
                        ecfg.PrefetchCount = Convert.ToUInt16(massTransitOptions[nameof(MassTransitOptions.ReceiveEndpointPrefetchCount)]);
                        ecfg.ConfigureConsumers(provider);
                        ecfg.UseMessageRetry(r => r.Interval(5, 1000));
                        ecfg.UseJsonSerializer();
                    });
                }));
            });

            services.AddMassTransitHostedService();
            services.AddCors();
            services.AddRouting(options => options.LowercaseUrls = true);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
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

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/healthz", new HealthCheckOptions()
                {
                    Predicate = _ => true,
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });
                endpoints.MapHealthChecksUI();

                endpoints.MapGet("/", async context =>
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
}
