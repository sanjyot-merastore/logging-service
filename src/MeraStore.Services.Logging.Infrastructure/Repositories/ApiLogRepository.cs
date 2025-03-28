using MeraStore.Services.Logging.Domain.Models;
using MeraStore.Services.Logging.Domain.Repositories;

namespace MeraStore.Services.Logging.Infrastructure.Repositories;

public class ApiLogRepository(AppDbContext dbContext) : IApiLogRepository
{

  public async Task AddRequestLogAsync(ApiRequestLog log, CancellationToken ct)
  {
    await dbContext.Set<ApiRequestLog>().AddAsync(log, ct);
    await dbContext.SaveChangesAsync(ct);
  }
  public async Task AddResponseLogAsync(ApiResponseLog log, CancellationToken ct)
  {
    await dbContext.Set<ApiResponseLog>().AddAsync(log, ct);
    await dbContext.SaveChangesAsync(ct);
  }

  public async Task<ApiRequestLog?> GetRequestLogByIdAsync(string id, CancellationToken ct)
  {
    return await dbContext.Set<ApiRequestLog>().FindAsync([id], ct);
  }
  public async Task<ApiResponseLog?> GetResponseLogByIdAsync(string id, CancellationToken ct)
  {
    return await dbContext.Set<ApiResponseLog>().FindAsync([id], ct);
  }
}