using MeraStore.Services.Logging.SDK.Models;

namespace MeraStore.Services.Logging.SDK.Interfaces;

public interface ILoggingApiClient
{
  Task<ApiResponse<string>> CreateRequestLogAsync(BaseDto command);
  Task<ApiResponse<string>> CreateResponseLogAsync(BaseDto command);
  Task<ApiResponse<LogFields>> GetLoggingFieldsAsync();
  Task<ApiResponse<RequestLog>> GetRequestLogAsync(Ulid id);
  Task<ApiResponse<ResponseLog>> GetResponseLogAsync(Ulid id);
}
