using Exchange.Rates.CoinCap.OpenApi.Extensions;
using Exchange.Rates.CoinCap.OpenApi.Filters;
using Exchange.Rates.CoinCap.OpenApi.Options;
using Exchange.Rates.Contracts.Messages;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using System;

namespace Exchange.Rates.CoinCap.OpenApi
{
    public class Startup
    {
        private const string SERVICE_NAME = "Exchange.Rates.CoinCap.OpenApi";

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

            // Register the Swagger generator
            services.AddSwaggerGen(options =>
            {
	            // Enable Swagger annotations
	            options.EnableAnnotations();

	            // Application Controller's API document description information
	            options.DocumentFilter<SwaggerDocumentFilter>();
            });

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
				x.AddRequestClient<SubmitCoinCapAssetId>();
			});

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

            app.UseEndpoints(configure =>
            {
                configure.MapControllers();
                configure.MapDefaultControllerRoute();
                // Redirect root to Swagger UI
                configure.MapGet("", context =>
                {
	                context.Response.Redirect("./swagger/index.html", permanent: false);
	                return Task.FromResult(0);
                });
            });
        }
    }
}
