using MeraStore.Services.Logging.SDK.Models;
namespace MeraStore.Services.Logging.SDK.Interfaces;

public interface ILoggingApiClient
{
  Task<ApiResponse<RequestLog>> CreateRequestLogAsync(RequestLog command,
    IList<KeyValuePair<string, string>> headers = null);
  Task<ApiResponse<ResponseLog>> CreateResponseLogAsync(ResponseLog command, IList<KeyValuePair<string, string>> headers = null);
  Task<ApiResponse<LoggingFields>> GetLoggingFieldsAsync(IList<KeyValuePair<string, string>> headers=null);
  Task<ApiResponse<RequestLog>> GetRequestLogAsync(Ulid id, IList<KeyValuePair<string, string>> headers = null);
  Task<ApiResponse<ResponseLog>> GetResponseLogAsync(Ulid id, IList<KeyValuePair<string, string>> headers = null);
}
