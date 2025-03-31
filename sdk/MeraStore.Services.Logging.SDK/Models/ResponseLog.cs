namespace MeraStore.Services.Logging.SDK.Models;

public class ResponseLog : BaseDto
{
  public int StatusCode { get; set; }
  public Guid RequestId { get; set; } // Link back to the original request
}