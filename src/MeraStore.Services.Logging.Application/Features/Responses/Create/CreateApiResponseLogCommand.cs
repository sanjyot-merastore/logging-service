using FastEndpoints;

using MeraStore.Services.Logging.Domain.Models;

namespace MeraStore.Services.Logging.Application.Features.Responses.Create;

public record CreateApiResponseLogCommand(int StatusCode, Guid RequestId, byte[] Payload, string ContentType, string CorrelationId)
  : ICommand<ApiResponseLog>;