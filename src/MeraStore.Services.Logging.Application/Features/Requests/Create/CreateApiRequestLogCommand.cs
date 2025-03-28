using FastEndpoints;

using MeraStore.Services.Logging.Domain.Models;

namespace MeraStore.Services.Logging.Application.Features.Requests.Create;

public record CreateApiRequestLogCommand(string HttpMethod, string Url, byte[] Payload, string ContentType, string CorrelationId)
  : ICommand<ApiRequestLog>;