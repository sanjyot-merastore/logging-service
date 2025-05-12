using FastEndpoints;

using MeraStore.Services.Logging.Domain.Models;
using MeraStore.Services.Logging.Domain.Repositories;

namespace MeraStore.Services.Logging.Application.Features.Requests.Create;

public class CreateApiRequestLogHandler(IApiLogRepository logRepo) : ICommandHandler<CreateApiRequestLogCommand, ApiRequestLog>
{

  public async Task<ApiRequestLog> ExecuteAsync(CreateApiRequestLogCommand cmd, CancellationToken ct)
  {
    var logEntry = new ApiRequestLog();
    try
    {
      
      logEntry.HttpMethod = cmd.HttpMethod;
      logEntry.Url = cmd.Url;
      logEntry.Payload = cmd.Payload;
      logEntry.ContentType = cmd.ContentType;
      logEntry.CorrelationId = cmd.CorrelationId;
      logEntry.Timestamp = DateTime.UtcNow;

      await logRepo.AddRequestLogAsync(logEntry, ct);
    }
    catch (Exception e)
    {
      Console.WriteLine(e);
      throw;
    }
    return logEntry;
  }
}