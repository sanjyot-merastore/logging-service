namespace MeraStore.Services.Logging.Domain.Models;

public class ApiResponseLog : ApiLogEntry
{
  public int StatusCode { get; set; }
  public Guid RequestId { get; set; } // Link back to the original request
}