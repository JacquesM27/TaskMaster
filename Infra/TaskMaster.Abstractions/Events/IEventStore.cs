namespace TaskMaster.Abstractions.Events;

// For Domain Events (Event Sourcing within aggregates)
public interface IDomainEventStore
{
    Task SaveAsync<T>(T @event, CancellationToken cancellationToken = default) where T : IDomainEvent;
    Task<IEnumerable<IDomainEvent>> GetEventsAsync(Guid aggregateId, CancellationToken cancellationToken = default);
    Task<IEnumerable<IDomainEvent>> GetEventsAsync(Guid aggregateId, int fromVersion, CancellationToken cancellationToken = default);
    Task<T?> GetLatestSnapshotAsync<T>(Guid aggregateId, CancellationToken cancellationToken = default) where T : class;
}

// For Integration Events (Cross-module communication)
public interface IIntegrationEventStore
{
    Task SaveAsync<T>(T @event, CancellationToken cancellationToken = default) where T : IIntegrationEvent;
    Task<IEnumerable<IIntegrationEvent>> GetEventsByTypeAsync(string eventType, CancellationToken cancellationToken = default);
    Task<IEnumerable<IIntegrationEvent>> GetEventsByTypeAsync(string eventType, DateTime from, DateTime to, CancellationToken cancellationToken = default);
    Task<IEnumerable<IIntegrationEvent>> GetEventsAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default);
}