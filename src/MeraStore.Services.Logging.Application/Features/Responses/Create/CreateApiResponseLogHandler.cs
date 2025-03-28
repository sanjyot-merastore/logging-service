using FastEndpoints;

using MeraStore.Services.Logging.Domain.Models;
using MeraStore.Services.Logging.Domain.Repositories;

namespace MeraStore.Services.Logging.Application.Features.Responses.Create;

public class CreateApiResponseLogHandler(IApiLogRepository logRepo) : ICommandHandler<CreateApiResponseLogCommand, ApiResponseLog>
{

  public async Task<ApiResponseLog> ExecuteAsync(CreateApiResponseLogCommand cmd, CancellationToken ct)
  {
    var logEntry = new ApiResponseLog
    {
      StatusCode = cmd.StatusCode,
      RequestId = cmd.RequestId,
      Payload = cmd.Payload,
      ContentType = cmd.ContentType,
      CorrelationId = cmd.CorrelationId,
      Timestamp = DateTime.UtcNow
    };

    await logRepo.AddResponseLogAsync(logEntry, ct);
    return logEntry;
  }
}