namespace MeraStore.Services.Logging.SDK.Models;

public class RequestLog : BaseDto
{
  public string HttpMethod { get; set; } = "GET";
  public string Url { get; set; } = string.Empty;
}