using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using TaskMaster.Abstractions.Events;
using TaskMaster.Infrastructure.DAL;

namespace TaskMaster.Infrastructure.Events;

// Domain Event Store - for event sourcing within aggregates
internal sealed class DomainEventStore(TaskMasterDbContext context) : IDomainEventStore
{
    public async Task SaveAsync<T>(T @event, CancellationToken cancellationToken = default) where T : IDomainEvent
    {
        var eventData = new DomainEventData
        {
            Id = Guid.NewGuid(),
            EventType = @event.GetType().FullName!,
            AggregateId = @event.AggregateId,
            Data = JsonSerializer.Serialize(@event),
            Version = GetEventVersion(@event),
            CreatedAt = @event.OccurredOn
        };

        await context.DomainEvents.AddAsync(eventData, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<IDomainEvent>> GetEventsAsync(Guid aggregateId, CancellationToken cancellationToken = default)
    {
        var events = await context.DomainEvents
            .Where(e => e.AggregateId == aggregateId)
            .OrderBy(e => e.Version)
            .ToListAsync(cancellationToken);

        return events.Select(DeserializeDomainEvent).Where(e => e != null)!;
    }

    public async Task<IEnumerable<IDomainEvent>> GetEventsAsync(Guid aggregateId, int fromVersion, CancellationToken cancellationToken = default)
    {
        var events = await context.DomainEvents
            .Where(e => e.AggregateId == aggregateId && e.Version >= fromVersion)
            .OrderBy(e => e.Version)
            .ToListAsync(cancellationToken);

        return events.Select(DeserializeDomainEvent).Where(e => e != null)!;
    }

    public async Task<T?> GetLatestSnapshotAsync<T>(Guid aggregateId, CancellationToken cancellationToken = default) where T : class
    {
        // Snapshot implementation would go here
        await Task.CompletedTask;
        return null;
    }

    private static IDomainEvent? DeserializeDomainEvent(DomainEventData eventData)
    {
        try
        {
            var eventType = Type.GetType(eventData.EventType);
            if (eventType == null) return null;

            return JsonSerializer.Deserialize(eventData.Data, eventType) as IDomainEvent;
        }
        catch
        {
            return null;
        }
    }

    private static int GetEventVersion(IDomainEvent @event)
    {
        // Simple version strategy - could be enhanced
        return 1;
    }
}

// Integration Event Store - for cross-module communication
internal sealed class IntegrationEventStore(TaskMasterDbContext context) : IIntegrationEventStore
{
    public async Task SaveAsync<T>(T @event, CancellationToken cancellationToken = default) where T : IIntegrationEvent
    {
        var eventData = new IntegrationEventData
        {
            Id = Guid.NewGuid(),
            EventType = @event.EventType,
            Data = JsonSerializer.Serialize(@event),
            Version = @event.Version,
            CreatedAt = @event.PublishedAt
        };

        await context.IntegrationEvents.AddAsync(eventData, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<IIntegrationEvent>> GetEventsByTypeAsync(string eventType, CancellationToken cancellationToken = default)
    {
        var events = await context.IntegrationEvents
            .Where(e => e.EventType == eventType)
            .OrderBy(e => e.CreatedAt)
            .ToListAsync(cancellationToken);

        return events.Select(DeserializeIntegrationEvent).Where(e => e != null)!;
    }

    public async Task<IEnumerable<IIntegrationEvent>> GetEventsByTypeAsync(string eventType, DateTime from, DateTime to, CancellationToken cancellationToken = default)
    {
        var events = await context.IntegrationEvents
            .Where(e => e.EventType == eventType && e.CreatedAt >= from && e.CreatedAt <= to)
            .OrderBy(e => e.CreatedAt)
            .ToListAsync(cancellationToken);

        return events.Select(DeserializeIntegrationEvent).Where(e => e != null)!;
    }

    public async Task<IEnumerable<IIntegrationEvent>> GetEventsAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default)
    {
        var events = await context.IntegrationEvents
            .Where(e => e.CreatedAt >= from && e.CreatedAt <= to)
            .OrderBy(e => e.CreatedAt)
            .ToListAsync(cancellationToken);

        return events.Select(DeserializeIntegrationEvent).Where(e => e != null)!;
    }

    private static IIntegrationEvent? DeserializeIntegrationEvent(IntegrationEventData eventData)
    {
        try
        {
            var eventType = Type.GetType(eventData.EventType);
            if (eventType == null) return null;

            return JsonSerializer.Deserialize(eventData.Data, eventType) as IIntegrationEvent;
        }
        catch
        {
            return null;
        }
    }
}

// Domain Events - for event sourcing within aggregates
internal sealed class DomainEventData
{
    public Guid Id { get; set; }
    public string EventType { get; set; } = string.Empty;
    public Guid AggregateId { get; set; }
    public string Data { get; set; } = string.Empty;
    public int Version { get; set; }
    public DateTime CreatedAt { get; set; }
}

// Integration Events - for cross-module communication
internal sealed class IntegrationEventData
{
    public Guid Id { get; set; }
    public string EventType { get; set; } = string.Empty;
    public string Data { get; set; } = string.Empty;
    public int Version { get; set; }
    public DateTime CreatedAt { get; set; }
}