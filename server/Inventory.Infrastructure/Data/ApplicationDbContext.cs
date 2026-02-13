using Inventory.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }
    public DbSet<AuditLogV2> AuditLogsV2 { get; set; }
    public DbSet<AuditLogV3> AuditLogsV3 { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Username).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.PasswordHash).IsRequired();
            entity.Property(e => e.Role).IsRequired().HasMaxLength(50);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
        });

        // Product configuration
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");
        });

        // AuditLog configuration
        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Action).IsRequired().HasMaxLength(100);
            entity.Property(e => e.EntityName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Timestamp).HasDefaultValueSql("GETUTCDATE()");
            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // AuditLogV2 configuration
        modelBuilder.Entity<AuditLogV2>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.UserId).IsRequired().HasMaxLength(100);
            entity.Property(e => e.UserName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.ActionType).IsRequired().HasMaxLength(100);
            entity.Property(e => e.EntityName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.EntityId).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Message).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Timestamp).HasDefaultValueSql("GETUTCDATE()");
            entity.Property(e => e.IpAddress).HasMaxLength(50);
            
            // Indexes for performance
            entity.HasIndex(e => e.Timestamp);
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.EntityId);
            entity.HasIndex(e => e.EntityName);
        });

        // AuditLogV3 configuration - Enterprise Jira-style audit logging
        modelBuilder.Entity<AuditLogV3>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            // Event Information
            entity.Property(e => e.EventType).IsRequired().HasMaxLength(100);
            entity.Property(e => e.EntityType).IsRequired().HasMaxLength(100);
            entity.Property(e => e.EntityId).IsRequired().HasMaxLength(100);
            entity.Property(e => e.EntityName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Action).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Description).HasMaxLength(500);
            
            // Change Tracking (JSON)
            entity.Property(e => e.OldValues).HasColumnType("nvarchar(max)");
            entity.Property(e => e.NewValues).HasColumnType("nvarchar(max)");
            entity.Property(e => e.ChangesSummary).HasMaxLength(1000);
            
            // User Information
            entity.Property(e => e.UserId).IsRequired();
            entity.Property(e => e.Username).IsRequired().HasMaxLength(100);
            entity.Property(e => e.UserEmail).IsRequired().HasMaxLength(200);
            
            // Request Context
            entity.Property(e => e.IpAddress).HasMaxLength(50);
            entity.Property(e => e.UserAgent).HasMaxLength(500);
            entity.Property(e => e.Source).IsRequired().HasMaxLength(100);
            
            // Metadata
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            entity.Property(e => e.CorrelationId).HasMaxLength(100);
            entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Severity).IsRequired().HasMaxLength(50);
            
            // Indexes for high-performance querying
            entity.HasIndex(e => e.CreatedAt).HasDatabaseName("IX_AuditLogsV3_CreatedAt");
            entity.HasIndex(e => e.UserId).HasDatabaseName("IX_AuditLogsV3_UserId");
            entity.HasIndex(e => e.EntityId).HasDatabaseName("IX_AuditLogsV3_EntityId");
            entity.HasIndex(e => e.EventType).HasDatabaseName("IX_AuditLogsV3_EventType");
            entity.HasIndex(e => e.Severity);
            entity.HasIndex(e => e.CorrelationId);
        });
    }
}
