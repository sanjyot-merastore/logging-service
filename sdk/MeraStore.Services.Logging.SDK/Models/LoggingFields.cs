using Newtonsoft.Json;

namespace MeraStore.Services.Logging.SDK.Models;

public class LoggingFields
{
  [JsonProperty("mappings")]
  public Mapping Mappings { get; set; } = new();
}

public class Mapping
{
  [JsonProperty("properties")]
  public Dictionary<string, PropertyType> Properties { get; set; } = new()
  {
    // General Metadata
    { "correlation-id", new PropertyType { Type = "keyword" } },
    { "txn-id", new PropertyType { Type = "keyword" } },
    { "trace-id", new PropertyType { Type = "keyword" } },
    { "span-id", new PropertyType { Type = "keyword" } },
    { "request-id", new PropertyType { Type = "keyword" } },
    { "parent-span-id", new PropertyType { Type = "keyword" } },
    { "tenant-id", new PropertyType { Type = "keyword" } },
    { "service-name", new PropertyType { Type = "keyword" } },
    { "environment", new PropertyType { Type = "keyword" } },
    { "host-name", new PropertyType { Type = "keyword" } },
    { "pod-name", new PropertyType { Type = "keyword" } },
    { "container-id", new PropertyType { Type = "keyword" } },

    // Network & Client Details
    { "server-ip", new PropertyType { Type = "ip" } },
    { "client-ip", new PropertyType { Type = "ip" } },
    { "user-agent", new PropertyType { Type = "text" } },

    // Exception Logging
    { "ex-message", new PropertyType { Type = "text" } },
    { "exception", new PropertyType { Type = "text" } },
    { "inner-exception", new PropertyType { Type = "text" } },
    { "ex-stack-trace", new PropertyType { Type = "text" } },
    { "ex-type", new PropertyType { Type = "keyword" } },
    { "ex-details", new PropertyType { Type = "text" } },
    { "is-handled-exception", new PropertyType { Type = "boolean" } }
  };
}

public class PropertyType
{
  [JsonProperty("type")] public string Type { get; set; } = string.Empty;
}
