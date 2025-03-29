using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Elastic.Clients.Elasticsearch;
using Serilog.Core;
using Serilog.Events;

namespace MeraStore.Services.Logging.Domain.LogSinks;

[SuppressMessage("ReSharper", "UseObjectOrCollectionInitializer")]
public abstract class BaseElasticsearchSink : ILogEventSink
{
  protected readonly ElasticsearchClient Client;
  protected readonly string HostName;
  protected readonly string OsPlatform;
  protected readonly string OsVersion;
  protected readonly int ProcessId;

  protected BaseElasticsearchSink(string elasticsearchUrl, string indexFormat)
  {
    var settings = new ElasticsearchClientSettings(new Uri(elasticsearchUrl))
      .DefaultIndex($"{indexFormat}{DateTime.UtcNow:yyyy-MM}");
    Client = new ElasticsearchClient(settings);
    HostName = Environment.MachineName;
    OsPlatform = RuntimeInformation.OSDescription;
    OsVersion = Environment.OSVersion.ToString();
    ProcessId = Environment.ProcessId;
  }

  public abstract void Emit(LogEvent logEvent);

  protected static string GetFormattedValue(IReadOnlyDictionary<string, LogEventPropertyValue> properties, string key, string defaultValue = "")
  {
    if (properties.TryGetValue(key, out var value))
    {
      return value is ScalarValue { Value: string strValue } ? strValue : value.ToString()?.Trim('"');
    }
    return defaultValue;
  }
  protected Dictionary<string, object> GetCommonLogFields(LogEvent logEvent)
  {
    var sourceContext = logEvent.Properties.TryGetValue("SourceContext", out var contextValue)
      ? contextValue.ToString()?.Trim('"')
      : string.Empty;
    var fields = new Dictionary<string, object>();
    fields[Constants.Logging.LogFields.Timestamp] = logEvent.Timestamp.UtcDateTime;
    fields[Constants.Logging.LogFields.Level] = logEvent.Level.ToString();
    fields["service-name"] = Constants.ServiceName;
    fields[Constants.Logging.LogFields.SourceContext] = sourceContext;
    fields[Constants.Logging.SystemMetadata.MachineName] = HostName;
    fields[Constants.Logging.SystemMetadata.OsPlatform] = OsPlatform;
    fields[Constants.Logging.SystemMetadata.OsVersion] = OsVersion;
    fields[Constants.Logging.SystemMetadata.ProcessId] = ProcessId;
    fields[Constants.Logging.SystemMetadata.ThreadId] = Environment.CurrentManagedThreadId;
    fields[Constants.Logging.SystemMetadata.Environment] = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";
    fields[Constants.Logging.LogFields.Message] = logEvent.RenderMessage();

    fields[Constants.Logging.LogFields.CorrelationId] = GetFormattedValue(logEvent.Properties, Constants.Logging.LogFields.CorrelationId);
    fields[Constants.Logging.LogFields.TransactionId] = GetFormattedValue(logEvent.Properties, Constants.Logging.LogFields.TransactionId);
    fields[Constants.Logging.LogFields.RequestId] = GetFormattedValue(logEvent.Properties, Constants.Logging.LogFields.RequestId);

    if (logEvent.Exception == null)
    {
      return fields;
    }

    fields[Constants.Logging.ExceptionFields.Exception] = logEvent.Exception.ToString();
    fields[Constants.Logging.ExceptionFields.InnerException] = logEvent.Exception.InnerException?.ToString();
    fields[Constants.Logging.ExceptionFields.ExceptionMessage] = logEvent.Exception.Message;
    fields[Constants.Logging.ExceptionFields.StackTrace] = logEvent.Exception.StackTrace;
    fields[Constants.Logging.ExceptionFields.ExceptionType] = logEvent.Exception.GetType().FullName;
    fields[Constants.Logging.ExceptionFields.ExceptionDetails] = logEvent.Exception.ToString();

    return fields;
  }
}