using FastEndpoints;
using FastEndpoints.Swagger;
using MeraStore.Services.Logging.Application.Services;
using MeraStore.Services.Logging.Domain.Interfaces;
using MeraStore.Services.Logging.Domain.LoggingSinks;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddFastEndpoints();
builder.Services.SwaggerDocument();

// Configure Serilog
const string elasticsearchUrl = "http://localhost:9200";
Log.Logger = new LoggerConfiguration()
  .Enrich.FromLogContext()
  .WriteTo.Console()
  .WriteTo.Sink(new ElasticsearchLogSink(elasticsearchUrl))
  .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddScoped<IFieldService, FieldService>();


var app = builder.Build();
app.UseFastEndpoints();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseSwaggerGen();
}

app.UseSerilogRequestLogging();
app.UseHttpsRedirection();

app.Run();
