using FastEndpoints;
using MeraStore.Services.Logging.Application.Features.Responses.Create;
using MeraStore.Services.Logging.Domain.Models;

namespace MeraStore.Services.Logging.Api.Endpoints;

/// <summary>
/// Creates a new API response log entry.
/// Stores details of an incoming API response, including HTTP method, URL, payload, and correlation ID.
/// </summary>
/// <param name="handler"></param>
public class CreateApiResponseLogEndpoint(CreateApiResponseLogHandler handler)
    : Endpoint<CreateApiResponseLogCommand, ApiResponseLog>
{

    public override void Configure()
    {
        Post("responses");
        AllowAnonymous();
        Description(d =>
        {
            d.WithSummary("Creates a new API response log entry")
                .WithDescription(
                    "Stores details of an incoming API response, including HTTP method, URL, payload, and correlation ID.")
                .Produces<ApiResponseLog>(201, "application/json")
                .Produces(400)
                .Produces(500);
        });
    }
    public override async Task<ApiResponseLog> ExecuteAsync(CreateApiResponseLogCommand req, CancellationToken ct)
    {
        return await handler.ExecuteAsync(req, ct);
    }
}