#nullable enable
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace MeraStore.Services.Logging.SDK;

public static class ResponseHelper
{
  private static readonly JsonSerializerSettings JsonOptions = new()
  {
    NullValueHandling = NullValueHandling.Ignore,
    Formatting = Formatting.Indented,
    ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver(),
    Converters = { new Newtonsoft.Json.Converters.StringEnumConverter() }
  };

  public static async Task<ApiResponse<T>> GetResponseOrFault<T>(this HttpResponseMessage response)
  {
    var content = await response.Content.ReadAsStringAsync();

    if (response.IsSuccessStatusCode)
    {
      return new ApiResponse<T>
      {
        Response = JsonConvert.DeserializeObject<T>(content, JsonOptions),
        StatusCode = (int)response.StatusCode
      };
    }

    return new ApiResponse<T>
    {
      StatusCode = (int)response.StatusCode,
      ErrorInfo = JsonConvert.DeserializeObject<ProblemDetails>(content, JsonOptions)
    };
  }
}