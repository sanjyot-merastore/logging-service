namespace MeraStore.Services.Logging.Domain.Models;

public class LogFields
{
  public Dictionary<string, string> Fields { get; set; } = new()
  {
    {"correlation-id","keyword"},
    {"trace-id","keyword"},
  };
}