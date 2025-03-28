using FastEndpoints;
using FastEndpoints.Swagger;

using MeraStore.Services.Logging.Api.Middlewares;
using MeraStore.Services.Logging.Application.Services;
using MeraStore.Services.Logging.Domain.Interfaces;
using MeraStore.Services.Logging.Domain.LoggingSinks;

using Serilog.Formatting.Compact;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddFastEndpoints();

builder.Services.SwaggerDocument(x =>
{
  x.ShortSchemaNames = true;
  x.NewtonsoftSettings = setting =>
  {
    setting.Formatting = Formatting.Indented;
    setting.ContractResolver = new CamelCasePropertyNamesContractResolver();
    setting.NullValueHandling = NullValueHandling.Ignore;
    setting.Converters =
    [
      new StringEnumConverter()
    ];
  };
});

// Configure Serilog
string elasticsearchUrl = builder.Configuration.GetValue<string>(Constants.Logging.Elasticsearch.Url)!;
Log.Logger = new LoggerConfiguration()
  .Enrich.FromLogContext()
  .WriteTo.Console()
  .WriteTo.Sink(new ElasticsearchSerilogSink(elasticsearchUrl))
  .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddSingleton<ILogFieldsProvider, LogFieldsProvider>();


var app = builder.Build();
app.UseFastEndpoints();
app.UseSwaggerGen();


app.UseMiddleware<TracingMiddleware>();
app.UseHttpsRedirection();

app.Run();
