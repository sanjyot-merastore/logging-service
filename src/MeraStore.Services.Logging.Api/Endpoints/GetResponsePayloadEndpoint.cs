using FastEndpoints;
using MeraStore.Services.Logging.Domain.Models;
using MeraStore.Services.Logging.Domain.Repositories;

using System.Text;

namespace MeraStore.Services.Logging.Api.Endpoints;

/// <summary>
/// Endpoint for retrieving the payload of a logged API response by its unique identifier.
/// </summary>
/// <param name="logService">Service for accessing API response logs.</param>
public class GetResponsePayloadEndpoint(IApiLogRepository logService) : EndpointWithoutRequest<string>
{
  public override void Configure()
  {
    Get("responses/payload/{id}");
    AllowAnonymous();
    Description(b =>
    {
      b.WithName("GetResponsePayload")
        .WithSummary("Retrieves the payload of a logged API response")
        .WithDescription("Returns the payload of an API response log by its unique identifier.")
        .Produces<ApiResponseLog>(200, "application/json")
        .Produces(404)
        .Produces(500);
    });
  }

  public override async Task HandleAsync(CancellationToken ct)
  {
    var id = Route<string>("id");
    var logEntry = await logService.GetResponseLogByIdAsync(id, ct);

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