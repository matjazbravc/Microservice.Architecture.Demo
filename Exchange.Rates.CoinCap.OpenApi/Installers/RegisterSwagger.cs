using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.OpenApi.Models;
using Exchange.Rates.CoinCap.OpenApi.Contracts;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Exchange.Rates.CoinCap.OpenApi.Installers
{
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
                var xmlDocFile = Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, xmlFile);
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
}
