namespace MeraStore.Services.Logging.Domain.Models;

public class LogFields
{
  public Dictionary<string, string> Fields { get; set; } = new()
  {
    { "correlation-id", "keyword" },
    { "txn-id", "keyword" },
    { "span-id", "keyword" },
    { "request-id", "keyword" },
    { "service-name", "keyword" },
    { "source-context", "keyword" },
    { "environment", "keyword" },
    { "host-name", "keyword" },
    { "ex-message", "text" },
    { "exception", "text" },
    { "inner-exception", "text" },
    { "ex-stack-trace", "text" },
    { "ex-type", "keyword" },
    { "ex-details", "text" }
  };
}