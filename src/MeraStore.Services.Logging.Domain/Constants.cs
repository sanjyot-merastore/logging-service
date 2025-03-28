namespace MeraStore.Services.Logging.Domain;

public static class Constants
{
  public const string ServiceName = "merastore-logging-service";

  public static class Logging
  {
    public static class Elasticsearch
    {
      public const string Url = "ElasticSearchUrl";
      public const string DefaultIndexFormat = "log-service-";
      public const string InfraIndexFormat = "infra-logs-";
    }

    public static class RequestHeaders
    {
      public const string CorrelationId = "ms-correlationId";
      public const string TraceId = "ms-traceId";
      public const string RequestId = "ms-requestId";
      public const string ClientIp = "ms-clientIp";
      public const string UserAgent = "ms-userAgent";
    }

    public static class LogFields
    {
      public const string Timestamp = "timestamp";
      public const string SourceContext = "source-context";
      public const string Level = "level";
      public const string Message = "message";
      public const string CorrelationId = "correlation-id";
      public const string TransactionId = "txn-id";
      public const string RequestId = "request-id";
      public const string RequestUrl = "request-url";
      public const string RequestBaseUrl = "request-base-url";
      public const string QueryString = "query-string";
      public const string HttpMethod = "http-method";
      public const string HttpVersion = "http-version";
      public const string StatusCode = "status-code";
      public const string ClientIp = "client-ip";
      public const string UserAgent = "user-agent";
      public const string RequestPath = "request-path";
    }

    public static class SystemMetadata
    {
      public const string MachineName = "machine-name";
      public const string OsPlatform = "os-platform";
      public const string OsVersion = "os-version";
      public const string ProcessId = "process-id";
      public const string ThreadId = "thread-id";
      public const string Environment = "environment";
    }

    public static class ExceptionFields
    {
      public const string Exception = "exception";
      public const string InnerException = "inner-exception";
      
      public const string ExceptionMessage = "ex-message";
      public const string StackTrace = "ex-stack-trace";
      public const string ExceptionType = "ex-type";
      public const string ExceptionDetails = "ex-details";
    }
  }
}
