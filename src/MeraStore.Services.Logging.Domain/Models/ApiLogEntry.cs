using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MeraStore.Services.Logging.Domain.Models;

public abstract class ApiLogEntry
{
  [Key]
  [DatabaseGenerated(DatabaseGeneratedOption.None)] // We generate ULID ourselves
  public string Id { get; set; } = Ulid.NewUlid().ToString();
  public byte[] Payload { get; set; } = []; // Raw request/response body
  public string ContentType { get; set; } = "text/plain"; // Helps in decoding the payload
  public DateTime Timestamp { get; set; } = DateTime.UtcNow;
  public string CorrelationId { get; set; } = string.Empty; // Links request and response
}