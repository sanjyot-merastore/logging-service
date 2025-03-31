using MeraStore.Services.Logging.SDK.Models;
namespace MeraStore.Services.Logging.SDK.Interfaces;

public interface ILoggingApiClient
{
  Task<ApiResponse<RequestLog>> CreateRequestLogAsync(RequestLog command);
  Task<ApiResponse<ResponseLog>> CreateResponseLogAsync(ResponseLog command);
  Task<ApiResponse<LogFields>> GetLoggingFieldsAsync();
  Task<ApiResponse<RequestLog>> GetRequestLogAsync(Ulid id);
  Task<ApiResponse<ResponseLog>> GetResponseLogAsync(Ulid id);
}
