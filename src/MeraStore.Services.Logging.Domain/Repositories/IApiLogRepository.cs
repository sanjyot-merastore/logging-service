using MeraStore.Services.Logging.Domain.Models;

namespace MeraStore.Services.Logging.Domain.Repositories;

public interface IApiLogRepository
{
  Task AddRequestLogAsync(ApiRequestLog log, CancellationToken ct);
  Task AddResponseLogAsync(ApiResponseLog log, CancellationToken ct);
  Task<ApiRequestLog?> GetRequestLogByIdAsync(string id, CancellationToken ct);
  Task<ApiResponseLog?> GetResponseLogByIdAsync(string id, CancellationToken ct);
}
