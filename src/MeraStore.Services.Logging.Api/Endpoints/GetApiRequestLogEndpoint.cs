using FastEndpoints;

using MeraStore.Services.Logging.Domain.Models;
using MeraStore.Services.Logging.Domain.Repositories;

namespace MeraStore.Services.Logging.Api.Endpoints;

/// <summary>
/// Endpoint for retrieving an API request log by its unique identifier.
/// </summary>
/// <param name="logService">Service for accessing API request logs.</param>
public class GetApiRequestLogEndpoint(IApiLogRepository logService) : EndpointWithoutRequest<ApiRequestLog>
{
  public override void Configure()
  {
    Get("requests/{id}"); // ✅ Ensures route consistency
    AllowAnonymous();
    Description(b =>
    {
      b.WithName("GetApiRequestLog")
        .WithSummary("Retrieves an API request log by ID")
        .WithDescription("Returns an API request log by its unique identifier.")
        .Produces<ApiResponseLog>(200, "application/json")
        .Produces(404)
        .Produces(500);
    });
  }

  public override async Task HandleAsync(CancellationToken ct)
  {
    var id = Route<string>("id"); // ✅ Extracts 'id' directly from the route

    var logEntry = await logService.GetRequestLogByIdAsync(id, ct);
    if (logEntry is null)
      await SendNotFoundAsync(ct);

    await SendOkAsync(logEntry!, ct);
  }
}