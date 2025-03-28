using Serilog.Events;

namespace MeraStore.Services.Logging.Domain.LoggingSinks;

public class InfraLogsElasticsearchSink(string elasticsearchUrl)
  : BaseElasticsearchSink(elasticsearchUrl, Constants.Logging.Elasticsearch.InfraIndexFormat)
{
  public override void Emit(LogEvent logEvent)
  {
    var logEntry = GetCommonLogFields(logEvent);

    Task.Run(async () => await Client.IndexAsync(logEntry, idx => idx.Index($"{Constants.Logging.Elasticsearch.InfraIndexFormat}{DateTime.UtcNow:yyyy-MM}")));
  }
}