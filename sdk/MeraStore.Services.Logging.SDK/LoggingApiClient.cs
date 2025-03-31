using System.Net.Http.Json;
using MeraStore.Services.Logging.SDK.Interfaces;
using MeraStore.Services.Logging.SDK.Models;

namespace MeraStore.Services.Logging.SDK;

internal record ResponseDto(string Id);

public class LoggingApiClient(HttpClient httpClient) : ILoggingApiClient
{
  public async Task<ApiResponse<RequestLog>> CreateRequestLogAsync(RequestLog command)
      => await PostLogAsync(ApiEndpoints.RequestLogs.Create, command);

  public async Task<ApiResponse<RequestLog>> GetRequestLogAsync(Ulid id)
      => await GetLogAsync<RequestLog>(ApiEndpoints.RequestLogs.Get, id);

  public async Task<ApiResponse<ResponseLog>> CreateResponseLogAsync(ResponseLog command)
      => await PostLogAsync(ApiEndpoints.ResponseLogs.Create, command);

  public async Task<ApiResponse<ResponseLog>> GetResponseLogAsync(Ulid id)
      => await GetLogAsync<ResponseLog>(ApiEndpoints.ResponseLogs.Get, id);

  public async Task<ApiResponse<LogFields>> GetLoggingFieldsAsync()
      => await GetLogAsync<LogFields>(ApiEndpoints.FieldLogs.GetAll);

  private async Task<ApiResponse<T>> PostLogAsync<T>(string endpoint, T command) where T: BaseDto
  {
    var response = await httpClient.PostAsJsonAsync(endpoint, command);
   return await response.GetResponseOrFault<T>();
   
  }

  private async Task<ApiResponse<T>> GetLogAsync<T>(string endpoint, Ulid? id = null)
  {
    var url = id.HasValue ? string.Format(endpoint, id) : endpoint;
    var response = await httpClient.GetAsync(url);
    return await response.GetResponseOrFault<T>();
  }
}
