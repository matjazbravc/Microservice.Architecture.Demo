using Exchange.Rates.Ecb.Polling.Api;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using System.IO;
using System.Threading.Tasks;

namespace Exchange.Rates.Ecb.Polling.Api;

public class Program
{
  public static async Task Main(string[] args)
  {
    await CreateHostBuilder(args).Build().RunAsync();
  }

  public static IHostBuilder CreateHostBuilder(string[] args) =>
   Host.CreateDefaultBuilder(args)
  .UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext())
  .UseContentRoot(Directory.GetCurrentDirectory())
  .UseDefaultServiceProvider(options => options.ValidateScopes = false)
  .ConfigureWebHostDefaults(webBuilder =>
  {
    webBuilder.UseStartup<Startup>()
      .UseKestrel()
      .UseWebRoot("wwwroot");
  })
  .ConfigureAppConfiguration((builderContext, config) =>
  {
    var env = builderContext.HostingEnvironment;
    config.SetBasePath(env.ContentRootPath);
    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
    config.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);
    config.AddEnvironmentVariables();
  })
  .ConfigureLogging(logging =>
  {
    logging.ClearProviders();
    logging.AddConsole();
    logging.AddSerilog();
  });
}