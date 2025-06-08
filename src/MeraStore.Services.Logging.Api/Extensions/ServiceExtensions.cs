using Elastic.Clients.Elasticsearch;

using FastEndpoints.Swagger;

using MeraStore.Services.Logging.Application.Features.Requests.Create;
using MeraStore.Services.Logging.Application.Features.Responses.Create;
using MeraStore.Services.Logging.Application.Services;
using MeraStore.Services.Logging.Domain.Interfaces;
using MeraStore.Services.Logging.Domain.LogSinks;
using MeraStore.Services.Logging.Domain.Repositories;
using MeraStore.Services.Logging.Infrastructure.Repositories;
using MeraStore.Shared.Kernel.Exceptions;

namespace MeraStore.Services.Logging.Api.Extensions;

/// <summary>
/// Provides extension methods for configuring application-level services like logging, Elasticsearch index templates, Swagger, and DI registrations.
/// </summary>
public static class ServiceExtensions
{
    /// <summary>
    /// Registers Elasticsearch client and the index template manager service used for bootstrapping index templates.
    /// </summary>
    /// <param name="services">The application services used for configuring services.</param>
    public static void AddIndexMangerServices(this IServiceCollection services, IConfiguration configuration)
    {
       services.AddSingleton(sp =>
        {
            var url = configuration.GetValue<string>("ElasticsearchUrl");

            if (string.IsNullOrWhiteSpace(url))
                throw LoggingServiceException.LogConfigurationMissing("ElasticsearchUrl is missing in config.");

            return new ElasticsearchClient(new ElasticsearchClientSettings(new Uri(url)));
        });

        services.AddSingleton<ILogIndexTemplateService, LogIndexTemplateService>();
    }

    /// <summary>
    /// Configures Serilog logging with Console output and Elasticsearch sinks for App, Infra, and EF logs.
    /// </summary>
    /// <param name="builder">The <see cref="WebApplicationBuilder"/> used to configure the application's logging pipeline.</param>
    /// <exception cref="InvalidOperationException">Thrown if Elasticsearch URL is missing in the application configuration.</exception>
    public static void AddLoggingServices(this WebApplicationBuilder builder)
    {
        // Retrieve Elasticsearch URL from configuration
        var elasticsearchUrl = builder.Configuration.GetValue<string>(Domain.Constants.Logging.Elasticsearch.Url);

        if (string.IsNullOrWhiteSpace(elasticsearchUrl))
        {
            throw new InvalidOperationException("Elasticsearch URL is missing in configuration.");
        }

        // Step 1: Create an Elastic client
        var elasticClient = new ElasticsearchClient(new ElasticsearchClientSettings(new Uri(elasticsearchUrl)));

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
    /// Adds Swagger/OpenAPI documentation support with XML docs and custom JSON formatting using Newtonsoft.
    /// </summary>
    /// <param name="services">The service collection to which Swagger is registered.</param>
    /// <returns>The modified service collection.</returns>
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
    /// Registers application-level services such as repositories, logging handlers, and utility providers used across the Logging API.
    /// </summary>
    /// <param name="services">The service collection to register dependencies into.</param>
    /// <returns>The updated <see cref="IServiceCollection"/> for chaining.</returns>
    public static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        services.AddSingleton<ILogFieldsProvider, LogFieldsProvider>();
        services.AddScoped<IApiLogRepository, ApiLogRepository>();

        services.AddScoped<CreateApiRequestLogHandler>();
        services.AddScoped<CreateApiResponseLogHandler>();

        return services;
    }
}
