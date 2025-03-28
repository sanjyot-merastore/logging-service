using FastEndpoints;

using MeraStore.Services.Logging.Domain.Interfaces;
using MeraStore.Services.Logging.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace MeraStore.Services.Logging.Api.Endpoints;

/// <summary>
/// Endpoint to retrieve the predefined logging fields schema allowed in Kibana logs.
/// </summary>
public class GetLoggingFieldsEndpoint(ILogFieldsProvider logService) : EndpointWithoutRequest<LogFields>
{
  /// <inheritdoc />
  public override void Configure()
  {
    Get("fields"); // ✅ Ensures route consistency
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

  /// <summary>
  /// Retrieves the predefined logging fields that are allowed in Kibana logs.
  /// </summary>
  /// <param name="ct">Cancellation token</param>
  /// <returns>A list of allowed logging fields.</returns>
  public override async Task HandleAsync(CancellationToken ct)
  {
    var fields = await logService.GetFieldsAsync();
    await SendAsync(fields, cancellation: ct);
  }
}