using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

namespace Exchange.Rates.Gateway
{
    public class Startup
    {
        private const string SERVICE_NAME = "Exchange.Rates.Gateway";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOcelot(Configuration);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseRouting();
            app.UseEndpoints(configure =>
            {
                configure.MapGet("", async context =>
                {
                    await context.Response.WriteAsync(SERVICE_NAME);
                });
            });
            app.UseStaticFiles();
            app.UseOcelot().Wait();
        }
    }
}
