using FastEndpoints.Swagger;

using MeraStore.Services.Logging.Application.Features.Requests.Create;
using MeraStore.Services.Logging.Application.Features.Responses.Create;
using MeraStore.Services.Logging.Application.Services;
using MeraStore.Services.Logging.Domain.Interfaces;
using MeraStore.Services.Logging.Domain.LogSinks;
using MeraStore.Services.Logging.Domain.Repositories;
using MeraStore.Services.Logging.Infrastructure.Repositories;


namespace MeraStore.Services.Logging.Api.Extensions;

/// <summary>
/// Provides extension methods for configuring logging services in the application.
/// </summary>
public static class ServiceExtensions
{
  /// <summary>
  /// Configures Serilog as the logging provider with multiple sinks, including Elasticsearch.
  /// </summary>
  /// <param name="builder">The <see cref="WebApplicationBuilder"/> used to configure services.</param>
  /// <exception cref="InvalidOperationException">Thrown if the Elasticsearch URL is missing in configuration.</exception>
  public static void AddLoggingServices(this WebApplicationBuilder builder)
  {
    // Retrieve Elasticsearch URL from configuration
    var elasticsearchUrl = builder.Configuration.GetValue<string>(Domain.Constants.Logging.Elasticsearch.Url);

    if (string.IsNullOrWhiteSpace(elasticsearchUrl))
    {
      throw new InvalidOperationException("Elasticsearch URL is missing in configuration.");
    }

    // Configure Serilog
    var logger = new LoggerConfiguration()
      .Enrich.FromLogContext()
      .WriteTo.Console(formatProvider: System.Globalization.CultureInfo.InvariantCulture) // Structured logging
      .WriteTo.Sink(new AppLogsElasticsearchSink(elasticsearchUrl))
      .WriteTo.Sink(new InfraLogsElasticsearchSink(elasticsearchUrl))
      .WriteTo.Sink(new EfLogsElasticsearchSink(elasticsearchUrl))
      .CreateLogger();

    // Assign Serilog as the logging provider
    Log.Logger = logger;
    builder.Host.UseSerilog();
  }

  /// <summary>
  /// Add Swagger document generation to the application.
  /// </summary>
  /// <param name="services"></param>
  /// <returns></returns>
  public static IServiceCollection AddSwaggerWithXmlDocs(this IServiceCollection services)
  {
    services.SwaggerDocument(config =>
    {
      config.ShortSchemaNames = true;
      config.EnableJWTBearerAuth = false;

      config.NewtonsoftSettings = settings =>
      {
        settings.Formatting = Formatting.Indented;
        settings.ContractResolver = new CamelCasePropertyNamesContractResolver();
        settings.NullValueHandling = NullValueHandling.Ignore;
        settings.Converters = [new StringEnumConverter()];
      };
    });

    return services;
  }

  /// <summary>
  /// 
  /// </summary>
  /// <param name="services"></param>
  /// <returns></returns>
  public static IServiceCollection AddApiServices(this IServiceCollection services)
  {
    services.AddSingleton<ILogFieldsProvider, LogFieldsProvider>();
    services.AddScoped<IApiLogRepository, ApiLogRepository>();

    services.AddScoped<CreateApiRequestLogHandler>();
    services.AddScoped<CreateApiResponseLogHandler>();


    return services;
  }
}
