using Elastic.Clients.Elasticsearch;

using Serilog.Core;
using Serilog.Events;

namespace DemoApi.LoggingSinks;

public class ElasticsearchLogSink : ILogEventSink
{
  private readonly ElasticsearchClient _client;

  public ElasticsearchLogSink(string elasticsearchUrl)
  {
    var settings = new ElasticsearchClientSettings(new Uri(elasticsearchUrl))
      .DefaultIndex($"app-logs-{DateTime.UtcNow:yyyy-MM}");

    _client = new ElasticsearchClient(settings);
  }

  public void Emit(LogEvent logEvent)
  {
    var logEntry = new Dictionary<string, object>
    {
      ["timestamp"] = logEvent.Timestamp.UtcDateTime,
      ["level"] = logEvent.Level.ToString(),
      ["cart_id"] = new Random().Next(1, 9999),
      ["app_name"] = "demo-app",
      ["rq_header.ms-correlationId"] = Guid.NewGuid(),
      ["message"] = logEvent.RenderMessage(),
      ["exception"] = logEvent.Exception?.ToString(),
      ["properties"] = logEvent.Properties
    };

    Task.Run(async () =>
    {
      try
      {
        var response = await _client.IndexAsync(logEntry, idx => idx.Index($"app-logs-{DateTime.UtcNow:yyyy-MM}"));
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