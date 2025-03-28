using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace MeraStore.Services.Logging.Application.Services;

public class ServiceHealthCheck : IHealthCheck
{
  public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
  {
    return Task.FromResult(HealthCheckResult.Healthy("Logging service is running."));
  }
}