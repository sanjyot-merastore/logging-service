using Serilog.Events;

namespace MeraStore.Services.Logging.Domain.LogSinks;

public class EfLogsElasticsearchSink(string elasticsearchUrl)
  : BaseElasticsearchSink(elasticsearchUrl, Constants.Logging.Elasticsearch.EFCoreIndexFormat)
{
  public override void Emit(LogEvent logEvent)
  {
    var logEntry = GetCommonLogFields(logEvent);
    var sourceContext = logEntry[Constants.Logging.LogFields.SourceContext]?.ToString();

    if (sourceContext == null || !sourceContext.Contains("Microsoft.EntityFrameworkCore"))
      return; // Ignore logs that don't belong here

    Task.Run(async () => await Client.IndexAsync(logEntry, idx => idx.Index($"{Constants.Logging.Elasticsearch.EFCoreIndexFormat}{DateTime.UtcNow:yyyy-MM}")));
  }
}