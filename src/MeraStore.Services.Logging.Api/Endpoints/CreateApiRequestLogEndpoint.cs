using FastEndpoints;
using MeraStore.Services.Logging.Application.Features.Requests.Create;
using MeraStore.Services.Logging.Domain.Models;

namespace MeraStore.Services.Logging.Api.Endpoints;

/// <summary>
/// Creates a new API request log entry
/// Stores details of an incoming API request, including HTTP method, URL, payload, and correlation ID.
/// </summary>
/// <param name="handler"></param>
public class CreateApiRequestLogEndpoint(CreateApiRequestLogHandler handler)
    : Endpoint<CreateApiRequestLogCommand, ApiRequestLog>
{

    public override void Configure()
    {
        Post("requests");
        AllowAnonymous();
        Description(d =>
        {
            d.WithSummary("Creates a new API request log entry")
                .WithDescription(
                    "Stores details of an incoming API request, including HTTP method, URL, payload, and correlation ID.")
                .Produces<ApiRequestLog>(201, "application/json")
                .Produces(400)
                .Produces(500);
        });
    }
    public override async Task<ApiRequestLog> ExecuteAsync(CreateApiRequestLogCommand req, CancellationToken ct)
    {
        return await handler.ExecuteAsync(req, ct);
    }
}