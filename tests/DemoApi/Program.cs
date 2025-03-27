using DemoApi.LoggingSinks;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
// Elasticsearch URL
const string elasticsearchUrl = "http://localhost:9200";

// Configure Serilog
Log.Logger = new LoggerConfiguration()
  .Enrich.FromLogContext()
  .WriteTo.Console()
  .WriteTo.Sink(new ElasticsearchLogSink(elasticsearchUrl))
  .CreateLogger();

builder.Host.UseSerilog();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseSerilogRequestLogging(); // Logs HTTP requests

app.Run();
