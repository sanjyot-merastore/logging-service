using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

using Elastic.Clients.Elasticsearch;

using Serilog.Core;
using Serilog.Events;

namespace MeraStore.Services.Logging.Domain.LoggingSinks;

[SuppressMessage("ReSharper", "UseObjectOrCollectionInitializer")]
public class ElasticsearchSerilogSink : ILogEventSink
{
  private readonly ElasticsearchClient _client;
  private readonly string _hostName;
  private readonly string _osPlatform;
  private readonly string _osVersion;
  private readonly int _processId;

  public ElasticsearchSerilogSink(string elasticsearchUrl)
  {
    var settings = new ElasticsearchClientSettings(new Uri(elasticsearchUrl))
      .DefaultIndex($"log-service-{DateTime.UtcNow:yyyy-MM}");

    _client = new ElasticsearchClient(settings);

    // Capture static system metadata
    _hostName = Environment.MachineName;
    _osPlatform = RuntimeInformation.OSDescription;
    _osVersion = Environment.OSVersion.ToString();
    _processId = Environment.ProcessId;
  }

  public void Emit(LogEvent logEvent)
  {
    var sourceContext = logEvent.Properties.TryGetValue("SourceContext", out var contextValue)
      ? contextValue.ToString()?.Trim('"')
      : string.Empty;

    var indexName = !string.IsNullOrEmpty(sourceContext) && sourceContext.StartsWith("MeraStore")
      ? $"log-service-{DateTime.UtcNow:yyyy-MM}"
      : $"{Constants.Logging.Elasticsearch.InfraIndexFormat}{DateTime.UtcNow:yyyy-MM}";

    var logEntry = new Dictionary<string, object>
    {
      [Constants.Logging.LogFields.Timestamp] = logEvent.Timestamp.UtcDateTime,
      [Constants.Logging.LogFields.Level] = logEvent.Level.ToString(),
      ["service-name"] = Constants.ServiceName,
      [Constants.Logging.LogFields.CorrelationId] = GetFormattedValue(logEvent.Properties, Constants.Logging.LogFields.CorrelationId),
      [Constants.Logging.LogFields.TransactionId] = GetFormattedValue(logEvent.Properties, Constants.Logging.LogFields.TransactionId),
      [Constants.Logging.LogFields.RequestId] = GetFormattedValue(logEvent.Properties, Constants.Logging.LogFields.RequestId),
      [Constants.Logging.LogFields.RequestUrl] = GetFormattedValue(logEvent.Properties, Constants.Logging.LogFields.RequestUrl),
      [Constants.Logging.LogFields.RequestBaseUrl] = GetFormattedValue(logEvent.Properties, Constants.Logging.LogFields.RequestBaseUrl),
      [Constants.Logging.LogFields.QueryString] = GetFormattedValue(logEvent.Properties, Constants.Logging.LogFields.QueryString),
      [Constants.Logging.LogFields.HttpMethod] = GetFormattedValue(logEvent.Properties, Constants.Logging.LogFields.HttpMethod),
      [Constants.Logging.LogFields.HttpVersion] = GetFormattedValue(logEvent.Properties, Constants.Logging.LogFields.HttpVersion),
      [Constants.Logging.LogFields.StatusCode] = GetFormattedValue(logEvent.Properties, Constants.Logging.LogFields.StatusCode),
      [Constants.Logging.LogFields.ClientIp] = GetFormattedValue(logEvent.Properties, Constants.Logging.LogFields.ClientIp),
      [Constants.Logging.LogFields.UserAgent] = GetFormattedValue(logEvent.Properties, Constants.Logging.LogFields.UserAgent),
      [Constants.Logging.LogFields.RequestPath] = GetFormattedValue(logEvent.Properties, Constants.Logging.LogFields.RequestPath),

      // System & Host Metadata
      [Constants.Logging.SystemMetadata.MachineName] = _hostName,
      [Constants.Logging.SystemMetadata.OsPlatform] = _osPlatform,
      [Constants.Logging.SystemMetadata.OsVersion] = _osVersion,
      [Constants.Logging.SystemMetadata.ProcessId] = _processId,
      [Constants.Logging.SystemMetadata.ThreadId] = Environment.CurrentManagedThreadId,
      [Constants.Logging.SystemMetadata.Environment] = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production",

      [Constants.Logging.LogFields.Message] = logEvent.RenderMessage(),
      [Constants.Logging.ExceptionFields.Exception] = logEvent.Exception?.ToString(),
      [Constants.Logging.ExceptionFields.InnerException] = logEvent.Exception?.InnerException?.ToString(),
      [Constants.Logging.LogFields.SourceContext] = sourceContext,
      [Constants.Logging.ExceptionFields.ExceptionMessage] = logEvent.Exception?.Message,
      [Constants.Logging.ExceptionFields.StackTrace] = logEvent.Exception?.StackTrace,
      [Constants.Logging.ExceptionFields.ExceptionType] = logEvent.Exception?.GetType().FullName,
      [Constants.Logging.ExceptionFields.ExceptionDetails] = logEvent.Exception?.ToString()
    };

    Task.Run(async () =>
    {
      try
      {
        var response = await _client.IndexAsync(logEntry, idx => idx.Index(indexName));
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

  private static string GetFormattedValue(IReadOnlyDictionary<string, LogEventPropertyValue> properties, string key, string defaultValue = "")
  {
    if (properties.TryGetValue(key, out var value))
    {
      return value is ScalarValue { Value: string strValue } ? strValue : value.ToString()?.Trim('"');
    }
    return defaultValue;
  }
}