﻿using Exchange.Rates.Ecb.OpenApi.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace Exchange.Rates.Ecb.OpenApi.Extensions;

internal static class ServiceRegistrationExtension
{
  public static void AddServicesInAssembly(this IServiceCollection services, IConfiguration configuration)
  {
    var appServices = typeof(Startup).Assembly.DefinedTypes
                    .Where(x => typeof(IServiceRegistration)
                    .IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
                    .Select(Activator.CreateInstance)
                    .Cast<IServiceRegistration>().ToList();
    appServices.ForEach(svc => svc.Register(services, configuration));
  }
}
