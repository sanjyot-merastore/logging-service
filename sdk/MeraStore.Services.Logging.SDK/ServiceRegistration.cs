using MeraStore.Services.Logging.SDK.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace MeraStore.Services.Logging.SDK;

public static class ServiceRegistration
{
  public static IServiceCollection AddLoggingSdk(this IServiceCollection services)
  {
    services.AddHttpClient();
    services.AddScoped<ILoggingApiClient, LoggingApiClient>();
    return services;
  }
}