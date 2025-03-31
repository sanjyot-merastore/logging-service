using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MeraStore.Services.Logging.SDK.Models;

public abstract class BaseDto
{
  [Key]
  [DatabaseGenerated(DatabaseGeneratedOption.None)] // We generate ULID ourselves
  public string Id { get; set; } = Ulid.NewUlid().ToString();
  public byte[] Payload { get; set; } = []; // Raw request/response body
  public string ContentType { get; set; } = "application/json"; // Helps in decoding the payload
  public DateTime Timestamp { get; set; } = DateTime.UtcNow;
  public string CorrelationId { get; set; } = string.Empty; // Links request and response
}