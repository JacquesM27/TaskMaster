using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskMaster.Abstractions.Outbox;
using TaskMaster.Infrastructure.Events;

namespace TaskMaster.Infrastructure.DAL;

internal sealed class TaskMasterDbContext(DbContextOptions<TaskMasterDbContext> options) : DbContext(options)
{
    public DbSet<OutboxMessage> OutboxMessages { get; set; }
    public DbSet<DomainEventData> DomainEvents { get; set; }
    public DbSet<IntegrationEventData> IntegrationEvents { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new OutboxMessageConfiguration());
        modelBuilder.ApplyConfiguration(new DomainEventDataConfiguration());
        modelBuilder.ApplyConfiguration(new IntegrationEventDataConfiguration());
    }
}

internal sealed class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.EventType).IsRequired().HasMaxLength(200);
        builder.Property(x => x.EventData).IsRequired();
        builder.Property(x => x.Source).IsRequired().HasMaxLength(100);
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.ProcessedAt);
        builder.Property(x => x.RetryCount);
        builder.Property(x => x.Error);

        builder.HasIndex(x => x.ProcessedAt);
        builder.HasIndex(x => x.CreatedAt);
        builder.HasIndex(x => x.EventType);
        
        builder.ToTable("OutboxMessages", "infrastructure");
    }
}

internal sealed class DomainEventDataConfiguration : IEntityTypeConfiguration<DomainEventData>
{
    public void Configure(EntityTypeBuilder<DomainEventData> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.EventType).IsRequired().HasMaxLength(500);
        builder.Property(x => x.AggregateId).IsRequired();
        builder.Property(x => x.Data).IsRequired();
        builder.Property(x => x.Version).IsRequired();
        builder.Property(x => x.CreatedAt).IsRequired();

        builder.HasIndex(x => x.AggregateId);
        builder.HasIndex(x => x.EventType);
        builder.HasIndex(x => x.CreatedAt);
        builder.HasIndex(x => new { x.AggregateId, x.Version }).IsUnique();
        
        builder.ToTable("DomainEvents", "infrastructure");
    }
}

internal sealed class IntegrationEventDataConfiguration : IEntityTypeConfiguration<IntegrationEventData>
{
    public void Configure(EntityTypeBuilder<IntegrationEventData> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.EventType).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Data).IsRequired();
        builder.Property(x => x.Version).IsRequired();
        builder.Property(x => x.CreatedAt).IsRequired();

        builder.HasIndex(x => x.EventType);
        builder.HasIndex(x => x.CreatedAt);
        builder.HasIndex(x => new { x.EventType, x.CreatedAt });
        
        builder.ToTable("IntegrationEvents", "infrastructure");
    }
}