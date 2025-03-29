using FastEndpoints;
using MeraStore.Services.Logging.Domain.Models;
using MeraStore.Services.Logging.Domain.Repositories;

namespace MeraStore.Services.Logging.Api.Endpoints;

/// <summary>
/// Endpoint for retrieving an API response log by its unique identifier.
/// </summary>
/// <param name="logService">Service for accessing API response logs.</param>
public class GetApiResponseLogEndpoint(IApiLogRepository logService) : EndpointWithoutRequest<ApiResponseLog>
{
  public override void Configure()
  {
    Get("responses/{id}"); // ✅ Ensures route consistency
    AllowAnonymous();
    Description(b =>
    {
      b.WithName("GetApiResponseLog")
        .WithSummary("Retrieves an API response log by ID")
        .WithDescription("Returns an API response log by its unique identifier.")
        .Produces<ApiResponseLog>(200, "application/json")
        .Produces(404)
        .Produces(500);
    });
  }
  public override async Task HandleAsync(CancellationToken ct)
  {
    var id = Route<string>("id"); // ✅ Extracts 'id' directly from the route

    var logEntry = await logService.GetResponseLogByIdAsync(id, ct);
    if (logEntry is null)
      await SendNotFoundAsync(ct);

    await SendOkAsync(logEntry!, ct);
  }
}