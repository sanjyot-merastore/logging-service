using Serilog.Events;

namespace MeraStore.Services.Logging.Domain.LoggingSinks;

public class EfLogsElasticsearchSink(string elasticsearchUrl)
  : BaseElasticsearchSink(elasticsearchUrl, Constants.Logging.Elasticsearch.EFCoreIndexFormat)
{
  public override void Emit(LogEvent logEvent)
  {
    var logEntry = GetCommonLogFields(logEvent);

    Task.Run(async () => await Client.IndexAsync(logEntry, idx => idx.Index($"{Constants.Logging.Elasticsearch.EFCoreIndexFormat}{DateTime.UtcNow:yyyy-MM}")));
  }
}