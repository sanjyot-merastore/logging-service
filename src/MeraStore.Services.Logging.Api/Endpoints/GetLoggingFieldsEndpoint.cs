using FastEndpoints;
using MeraStore.Services.Logging.Domain.Interfaces;
using MeraStore.Services.Logging.Domain.Models;

namespace MeraStore.Services.Logging.Api.Endpoints
{
  public class GetLoggingFieldsEndpoint(IFieldService logService) : EndpointWithoutRequest<LogFields>
  {
    public override void Configure()
    {
      Get("/api/v1.0/logfields");
      AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
      await SendAsync(await logService.GetFieldsAsync(), cancellation: ct);
    }
  }
}
