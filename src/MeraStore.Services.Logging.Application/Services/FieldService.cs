using MeraStore.Services.Logging.Domain.Interfaces;
using MeraStore.Services.Logging.Domain.Models;

using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

namespace MeraStore.Services.Logging.Application.Services;

public class FieldService(ILogger<FieldService> logger) : IFieldService
{
  public async Task<LogFields> GetFieldsAsync()
  {
    try
    {
      // Construct file path using platform-independent separator
      string filePath = Path.Combine(AppContext.BaseDirectory, "configs", "log-schema.json");

      if (!File.Exists(filePath))
      {
        logger.LogWarning("log-schema.json not found at {FilePath}", filePath);
        return new LogFields();
      }

      // Open file with FileShare.Read to allow multiple readers
      await using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
      using var reader = new StreamReader(stream);

      // Read content asynchronously
      string jsonContent = await reader.ReadToEndAsync();

      // Deserialize JSON safely
      var logFields = JsonConvert.DeserializeObject<LogFields>(jsonContent);

      return logFields ?? new LogFields(); // Return empty object if deserialization fails
    }
    catch (Exception ex)
    {
      logger.LogError(ex, "Error reading log-schema.json");
      return new LogFields(); // Return empty object in case of an error
    }
  }
}