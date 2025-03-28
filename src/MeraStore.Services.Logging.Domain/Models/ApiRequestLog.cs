namespace MeraStore.Services.Logging.Domain.Models;

public class ApiRequestLog : ApiLogEntry
{
  public string HttpMethod { get; set; } = "GET";
  public string Url { get; set; } = string.Empty;
}