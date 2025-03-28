using FastEndpoints;

using MeraStore.Services.Logging.Domain.Models;
using MeraStore.Services.Logging.Domain.Repositories;

namespace MeraStore.Services.Logging.Application.Features.Requests.Create;

public class CreateApiRequestLogHandler(IApiLogRepository logRepo) : ICommandHandler<CreateApiRequestLogCommand, ApiRequestLog>
{

  public async Task<ApiRequestLog> ExecuteAsync(CreateApiRequestLogCommand cmd, CancellationToken ct)
  {
    var logEntry = new ApiRequestLog
    {
      HttpMethod = cmd.HttpMethod,
      Url = cmd.Url,
      Payload = cmd.Payload,
      ContentType = cmd.ContentType,
      CorrelationId = cmd.CorrelationId,
      Timestamp = DateTime.UtcNow
    };

    await logRepo.AddRequestLogAsync(logEntry, ct);
    return logEntry;
  }
}