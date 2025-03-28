using MeraStore.Services.Logging.Domain.Models;

using Microsoft.EntityFrameworkCore;

namespace MeraStore.Services.Logging.Infrastructure;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
  public DbSet<ApiRequestLog> Requests { get; set; } = null!;
  public DbSet<ApiResponseLog> Responses { get; set; } = null!;

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);

    modelBuilder.Entity<ApiRequestLog>(entity =>
    {
      entity.HasKey(e => e.Id);
      entity.Property(e => e.Id).HasMaxLength(26); // ULIDs are 26 chars long
      entity.Property(e => e.HttpMethod).IsRequired().HasMaxLength(10);
      entity.Property(e => e.Url).IsRequired();
      entity.Property(e => e.ContentType).HasMaxLength(100);
      entity.Property(e => e.Payload).HasColumnType("VARBINARY(MAX)");
      entity.Property(e => e.Timestamp).HasDefaultValueSql("GETUTCDATE()"); // ✅ Fix for SQL Server

      // Indexes
      entity.HasIndex(e => e.CorrelationId);
      entity.HasIndex(e => e.Timestamp);
      entity.HasIndex(e => new { e.HttpMethod, e.Timestamp }); // Optimized for method & time-based queries
    });

    modelBuilder.Entity<ApiResponseLog>(entity =>
    {
      entity.HasKey(e => e.Id);
      entity.Property(e => e.Id).HasMaxLength(26);
      entity.Property(e => e.RequestId).IsRequired();
      entity.Property(e => e.StatusCode).IsRequired();
      entity.Property(e => e.ContentType).HasMaxLength(100);
      entity.Property(e => e.Payload).HasColumnType("VARBINARY(MAX)");
      entity.Property(e => e.Timestamp).HasDefaultValueSql("GETUTCDATE()"); // ✅ Fix for SQL Server

      // Indexes
      entity.HasIndex(e => e.CorrelationId);
      entity.HasIndex(e => e.Timestamp);
      entity.HasIndex(e => e.StatusCode);
      entity.HasIndex(e => new { e.RequestId, e.Timestamp }); // Fast lookup by request & time
    });
  }
}