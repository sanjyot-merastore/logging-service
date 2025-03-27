using Elastic.Clients.Elasticsearch;
using Serilog.Core;
using Serilog.Events;

namespace MeraStore.Services.Logging.Domain.LoggingSinks;

public class ElasticsearchLogSink : ILogEventSink
{
  private readonly ElasticsearchClient _client;

  public ElasticsearchLogSink(string elasticsearchUrl)
  {
    var settings = new ElasticsearchClientSettings(new Uri(elasticsearchUrl))
      .DefaultIndex($"log-service-{DateTime.UtcNow:yyyy-MM}");

    _client = new ElasticsearchClient(settings);
  }

  public void Emit(LogEvent logEvent)
  {
    var logEntry = new Dictionary<string, object>
    {
      ["timestamp"] = logEvent.Timestamp.UtcDateTime,
      ["level"] = logEvent.Level.ToString(),
      ["app_name"] = "merastore-logging-service",
      ["correlation-id"] = logEvent.Properties.TryGetValue("correlation-id", out var correlationId) ? correlationId.ToString() : Guid.NewGuid(),
      ["message"] = logEvent.RenderMessage(),
      ["exception"] = logEvent.Exception?.ToString(),
      ["innerexception"] = logEvent.Exception?.InnerException?.ToString(),
      
    };

    Task.Run(async () =>
    {
      try
      {
        var response = await _client.IndexAsync(logEntry, idx => idx.Index($"log-service-{DateTime.UtcNow:yyyy-MM}"));
        if (!response.IsValidResponse)
        {
          Console.WriteLine($"Failed to log to Elasticsearch: {response.DebugInformation}");
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine($"Error logging to Elasticsearch: {ex.Message}");
      }
    });
  }
}