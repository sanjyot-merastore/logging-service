using MeraStore.Services.Logging.Domain.Interfaces;
using MeraStore.Services.Logging.Domain.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace MeraStore.Services.Logging.Application.Services;

public class LogFieldsProvider : ILogFieldsProvider
{
  private readonly ILogger<LogFieldsProvider> _logger;
  private readonly Lazy<Task<LogFields>> _logFields;

  public LogFieldsProvider(ILogger<LogFieldsProvider> logger)
  {
    _logger = logger;
    _logFields = new Lazy<Task<LogFields>>(LoadFieldsAsync);
  }

  public async Task<LogFields> GetFieldsAsync()
  {
    return await _logFields.Value;
  }

  public async Task<LogFields> LoadFieldsAsync()
  {
    try
    {
      var filePath = Path.Combine(AppContext.BaseDirectory, "configs", "log-schema.json");

      if (!File.Exists(filePath))
      {
        _logger.LogWarning("log-schema.json not found at {FilePath}", filePath);
        return new LogFields();
      }

      await using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
      using var reader = new StreamReader(stream);
      var jsonContent = await reader.ReadToEndAsync();

      var logFields = JsonConvert.DeserializeObject<LogFields>(jsonContent);
      return logFields ?? new LogFields();
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error reading log-schema.json");
      return new LogFields();
    }
  }
}