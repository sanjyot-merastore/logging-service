using System.Net.Http.Json;

using MeraStore.Services.Logging.SDK.Interfaces;
using MeraStore.Services.Logging.SDK.Models;

namespace MeraStore.Services.Logging.SDK;

internal record ResponseDto(string Id);

public class LoggingApiClient(HttpClient httpClient) : ILoggingApiClient
{
  public async Task<ApiResponse<RequestLog>> CreateRequestLogAsync(RequestLog command,
    IList<KeyValuePair<string, string>> headers = null)
    => await PostLogAsync(ApiEndpoints.RequestLogs.Create, command, headers ?? []);

  public async Task<ApiResponse<RequestLog>> GetRequestLogAsync(Ulid id,
    IList<KeyValuePair<string, string>> headers = null)
    => await GetLogAsync<RequestLog>(ApiEndpoints.RequestLogs.Get, id, headers ?? []);

  public async Task<ApiResponse<ResponseLog>> CreateResponseLogAsync(ResponseLog command,
    IList<KeyValuePair<string, string>> headers = null)
    => await PostLogAsync(ApiEndpoints.ResponseLogs.Create, command, headers);

  public async Task<ApiResponse<ResponseLog>> GetResponseLogAsync(Ulid id, IList<KeyValuePair<string, string>> headers = null)
    => await GetLogAsync<ResponseLog>(ApiEndpoints.ResponseLogs.Get, id, headers ?? []);

  public async Task<ApiResponse<LoggingFields>> GetLoggingFieldsAsync(IList<KeyValuePair<string, string>> headers = null)
    => await GetLogAsync<LoggingFields>(ApiEndpoints.FieldLogs.GetAll, headers: headers ?? []);

  private async Task<ApiResponse<T>> PostLogAsync<T>(string endpoint, T command,
    IList<KeyValuePair<string, string>> headers = null) where T : BaseDto
  {
    AddHeaders(headers);
    var response = await httpClient.PostAsJsonAsync(endpoint, command);
    return await response.GetResponseOrFault<T>();
  }

  private async Task<ApiResponse<T>> GetLogAsync<T>(string endpoint, Ulid? id = null,
    IList<KeyValuePair<string, string>> headers = null)
  {
    var url = id.HasValue ? string.Format(endpoint, id) : endpoint;
    AddHeaders(headers);
    var response = await httpClient.GetAsync(url);
    return await response.GetResponseOrFault<T>();
  }

  private void AddHeaders(IList<KeyValuePair<string, string>> headers)
  {
    if (!headers.Any()) return;
    foreach (var header in headers)
    {
      httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
    }
  }
}