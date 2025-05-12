using FastEndpoints;
using MeraStore.Services.Logging.Domain.Models;
using MeraStore.Services.Logging.Domain.Repositories;
using System.Text;

namespace MeraStore.Services.Logging.Api.Endpoints;

/// <summary>
/// Endpoint for retrieving the payload of a logged API request by its unique identifier.
/// </summary>
/// <param name="logService">Service for accessing API request logs.</param>
public class GetRequestPayloadEndpoint(IApiLogRepository logService) : EndpointWithoutRequest<object>
{
  public override void Configure()
  {
    Get("requests/payload/{id}");
    AllowAnonymous();
    Description(b =>
    {
      b.WithName("GetRequestPayload")
        .WithSummary("Retrieves the payload of a logged API request")
        .WithDescription("Returns the payload of an API request log by its unique identifier.")
        .Produces<ApiResponseLog>(200, "application/json")
        .Produces(404)
        .Produces(500);
    });
  }

  public override async Task HandleAsync(CancellationToken ct)
  {
    var id = Route<string>("id");
    var logEntry = await logService.GetRequestLogByIdAsync(id, ct);

    if (logEntry?.Payload != null)
    {
      HttpContext.Response.ContentType = "application/json";
      await HttpContext.Response.Body.WriteAsync(logEntry.Payload, ct);
    }
    else
    {
      await SendNotFoundAsync(ct);
    }
  }
}