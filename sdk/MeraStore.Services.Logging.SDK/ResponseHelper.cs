#nullable enable
using MeraStore.Shared.Kernel.Common.Core;
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
    var isSuccess = response.IsSuccessStatusCode;

    if (isSuccess && !string.IsNullOrWhiteSpace(content))
    {
      try
      {
        T? responseData;

        if (typeof(T) == typeof(string))
        {
          // Handle string separately (avoid JSON deserialization issues)
          responseData = (T)(object)content;
        }
        else if (typeof(T).IsPrimitive || typeof(T) == typeof(decimal))
        {
          // Handle primitive types separately
          responseData = (T)Convert.ChangeType(content, typeof(T));
        }
        else
        {
          // Default: Deserialize JSON normally
          responseData = JsonConvert.DeserializeObject<T>(content, JsonOptions);
        }

        return new ApiResponse<T>
        {
          Response = responseData,
          StatusCode = (int)response.StatusCode
        };
      }
      catch (JsonException ex)
      {
        return new ApiResponse<T>
        {
          StatusCode = (int)response.StatusCode,
          ErrorInfo = new ProblemDetails
          {
            Title = "Deserialization Error",
            Detail = ex.Message,
            Status = (int)response.StatusCode
          }
        };
      }
    }

    if (!isSuccess && !string.IsNullOrWhiteSpace(content))
    {
      try
      {
        return new ApiResponse<T>
        {
          StatusCode = (int)response.StatusCode,
          ErrorInfo = JsonConvert.DeserializeObject<ProblemDetails>(content, JsonOptions)
        };
      }
      catch (JsonException ex)
      {
        return new ApiResponse<T>
        {
          StatusCode = (int)response.StatusCode,
          ErrorInfo = new ProblemDetails
          {
            Title = "Error Response Parsing Failed",
            Detail = ex.Message,
            Status = (int)response.StatusCode
          }
        };
      }
    }

    // Handle empty content
    return new ApiResponse<T>
    {
      StatusCode = (int)response.StatusCode,
      ErrorInfo = new ProblemDetails
      {
        Title = "No Content",
        Detail = "The API response did not contain any content.",
        Status = (int)response.StatusCode
      }
    };
  }


}