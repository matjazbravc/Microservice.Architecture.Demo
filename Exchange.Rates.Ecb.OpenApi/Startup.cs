using Exchange.Rates.Contracts.Messages;
using Exchange.Rates.Ecb.OpenApi.Extensions;
using Exchange.Rates.Ecb.OpenApi.Options;
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
using Microsoft.Extensions.Hosting;
using System;

namespace Exchange.Rates.Ecb.OpenApi
{
    public class Startup
    {
        private const string SERVICE_NAME = "Exchange.Rates.Ecb.OpenApi";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; set; }

        public virtual void ConfigureServices(IServiceCollection services)
        {
            // Add services required for using options
            services.AddOptions();

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

            // Register services in Installers folder
            services.AddServicesInAssembly(Configuration);

            // https://localhost:{port}/healthchecks-ui
            services.AddHealthChecksUI(opt =>
            {
                opt.SetEvaluationTimeInSeconds(15); // time in seconds between check
                opt.MaximumHistoryEntriesPerEndpoint(60); // maximum history of checks
                opt.SetApiMaxActiveRequests(1); // api requests concurrency
            })
            .AddInMemoryStorage();

            // Formats the endpoint names usink kebab-case (dashed snake case)
            services.TryAddSingleton(KebabCaseEndpointNameFormatter.Instance);

			// Add MassTransit support
			services.AddMassTransit(x =>
			{
				x.AddBus(_ => Bus.Factory.CreateUsingRabbitMq(config =>
				{
					config.Host(new Uri(massTransitOptions[nameof(MassTransitOptions.Host)]), h =>
					{
						h.Username(massTransitOptions[nameof(MassTransitOptions.Username)]);
						h.Password(massTransitOptions[nameof(MassTransitOptions.Password)]);
					});
				}));
				x.AddRequestClient<SubmitEcbExchangeRateSymbols>();
			});

			services.AddMassTransitHostedService();
            services.AddControllers()
                .ConfigureApiBehaviorOptions(options =>
                {
                    options.SuppressConsumesConstraintForFormFileParameters = true;
                    options.SuppressInferBindingSourcesForParameters = true;
                    options.SuppressModelStateInvalidFilter = true; // To disable the automatic 400 behavior, set the SuppressModelStateInvalidFilter property to true
                    options.SuppressMapClientErrors = true;
                    options.ClientErrorMapping[404].Link = "https://httpstatuses.com/404";
                })
                .AddNewtonsoftJson(options =>
                    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                );

            services.AddCors();
            services.AddRouting(options => options.LowercaseUrls = true);
        }

        public virtual void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", SERVICE_NAME);
            });

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
                endpoints.MapControllers();
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
    }
}
