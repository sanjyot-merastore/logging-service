using Serilog.Context;

namespace MeraStore.Services.Logging.Api.Middlewares;

/// <summary>
/// Middleware to ensure every request has traceability headers and propagates them across logs.
/// </summary>
public class TracingMiddleware(RequestDelegate next)
{
  /// <summary>
  /// Processes the request, ensuring traceability headers are present.
  /// </summary>
  /// <param name="context">HTTP request context.</param>
  public async Task InvokeAsync(HttpContext context)
  {
    // Retrieve or generate necessary IDs
    var correlationId = context.Request.Headers[Constants.Logging.RequestHeaders.CorrelationId].FirstOrDefault() ?? Guid.NewGuid().ToString();
    var traceId = context.Request.Headers[Constants.Logging.RequestHeaders.TraceId].FirstOrDefault() ?? Guid.NewGuid().ToString();
    var requestId = context.Request.Headers[Constants.Logging.RequestHeaders.RequestId].FirstOrDefault() ?? Guid.NewGuid().ToString();
    var clientIp = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    var userAgent = context.Request.Headers[Constants.Logging.RequestHeaders.UserAgent].FirstOrDefault() ?? "unknown";

    // Get full request URL (including scheme, host, and query params)
    var requestUrl = $"{context.Request.Scheme}://{context.Request.Host}{context.Request.Path}{context.Request.QueryString}";
    var requestBaseUrl = $"{context.Request.Scheme}://{context.Request.Host}";
    var httpMethod = context.Request.Method;
    var httpVersion = context.Request.Protocol;
    var requestPath = context.Request.Path;
    var queryString = context.Request.QueryString.ToString();

    // Add values to request headers if missing
    context.Request.Headers[Constants.Logging.RequestHeaders.CorrelationId] = correlationId;
    context.Request.Headers[Constants.Logging.RequestHeaders.TraceId] = traceId;
    context.Request.Headers[Constants.Logging.RequestHeaders.RequestId] = requestId;
    context.Request.Headers[Constants.Logging.RequestHeaders.ClientIp] = clientIp;
    context.Request.Headers[Constants.Logging.RequestHeaders.UserAgent] = userAgent;

    // Add properties to Serilog LogContext for structured logging
    using (LogContext.PushProperty(Constants.Logging.LogFields.CorrelationId, correlationId))
    using (LogContext.PushProperty(Constants.Logging.LogFields.TransactionId, traceId))
    using (LogContext.PushProperty(Constants.Logging.LogFields.RequestId, requestId))
    using (LogContext.PushProperty(Constants.Logging.LogFields.RequestBaseUrl, requestBaseUrl)) // NEW: Base URL
    using (LogContext.PushProperty(Constants.Logging.LogFields.RequestUrl, requestUrl)) // Full URL added here
    using (LogContext.PushProperty(Constants.Logging.LogFields.QueryString, queryString))
    using (LogContext.PushProperty(Constants.Logging.LogFields.HttpMethod, httpMethod))
    using (LogContext.PushProperty(Constants.Logging.LogFields.HttpVersion, httpVersion))
    using (LogContext.PushProperty(Constants.Logging.LogFields.RequestPath, requestPath))
    using (LogContext.PushProperty(Constants.Logging.LogFields.ClientIp, clientIp))
    using (LogContext.PushProperty(Constants.Logging.LogFields.UserAgent, userAgent))
    {
      // Ensure response headers contain traceability IDs
      context.Response.OnStarting(() =>
      {
        context.Response.Headers[Constants.Logging.RequestHeaders.CorrelationId] = correlationId;
        context.Response.Headers[Constants.Logging.RequestHeaders.TraceId] = traceId;
        context.Response.Headers[Constants.Logging.RequestHeaders.RequestId] = requestId;
        return Task.CompletedTask;
      });

      // Continue processing the request
      await next(context);
    }
  }
}