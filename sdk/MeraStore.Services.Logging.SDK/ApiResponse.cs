#nullable enable

using Microsoft.AspNetCore.Mvc;

namespace MeraStore.Services.Logging.SDK;

public class ApiResponse<T>
{
  public T? Response { get; set; }
  public int StatusCode { get; set; }
  public ProblemDetails? ErrorInfo { get; set; }

  public bool IsSuccess => StatusCode is >= 200 and < 300;
}