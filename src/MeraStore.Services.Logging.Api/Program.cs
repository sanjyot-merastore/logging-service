using Elastic.Clients.Elasticsearch;

using FastEndpoints;
using FastEndpoints.Swagger;

using MeraStore.Services.Logging.Api.Extensions;
using MeraStore.Services.Logging.Api.Middlewares;
using MeraStore.Services.Logging.Application.Services;
using MeraStore.Services.Logging.Domain.Interfaces;
using MeraStore.Services.Logging.Infrastructure;

using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddFastEndpoints();


builder.Services.AddHealthChecks()
  .AddCheck<ServiceHealthCheck>("logging_service_health_check");
builder.Services.AddDbContext<AppDbContext>(options =>
  options.UseSqlServer(builder.Configuration.GetConnectionString("LoggingDb")));

builder.Services.AddProblemDetails();
builder.Services.AddSwaggerWithXmlDocs();
builder.Services.AddIndexMangerServices(builder.Configuration);
builder.AddLoggingServices();

builder.Services.AddApiServices();

var app = builder.Build();
app.UseFastEndpoints(c =>
{
    c.Versioning.Prefix = "v";
    c.Endpoints.RoutePrefix = "api/v1.0/logs"; // Ensures endpoints are correctly mapped
});
app.UseSwaggerGen();

// Apply database migrations on startup with logging
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    var templateService = scope.ServiceProvider.GetRequiredService<ILogIndexTemplateService>();

    await RunMigrations(logger, dbContext);

    await templateService.SetupTemplatesAsync();
}


app.UseMiddleware<TracingMiddleware>();
app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseHttpsRedirection();
app.MapHealthChecks("/health");

app.Run();

async Task RunMigrations(ILogger<Program> logger, AppDbContext appDbContext)
{
    try
    {
        logger.LogInformation("Applying database migrations...");
        await appDbContext.Database.MigrateAsync();
        logger.LogInformation("✅ Database migrations applied successfully.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "❌ Error applying database migrations.");
    }
}

