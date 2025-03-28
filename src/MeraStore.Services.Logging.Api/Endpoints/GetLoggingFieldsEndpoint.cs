using FastEndpoints;
using MeraStore.Services.Logging.Domain.Interfaces;
using MeraStore.Services.Logging.Domain.Models;

namespace MeraStore.Services.Logging.Api.Endpoints
{
  /// <summary>
  /// Endpoint to retrieve the predefined logging fields schema allowed in Kibana logs.
  /// </summary>
  public class GetLoggingFieldsEndpoint(ILogFieldsProvider logService, ILogger<GetLoggingFieldsEndpoint> logger) : EndpointWithoutRequest<LogFields>
  {
    /// <inheritdoc />
    public override void Configure()
    {
      Get("/api/v1.0/logfields");
      AllowAnonymous();
      Description(b =>
      {
        b.WithName("GetLoggingFields")
          .WithSummary("Retrieves the logging fields schema")
          .WithDescription("Returns the predefined logging fields that are allowed in Kibana logs.")
          .Produces<LogFields>(200, "application/json")
          .Produces(500);
      });
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
      await SendAsync(await logService.GetFieldsAsync(), cancellation: ct);
    }
  }
}