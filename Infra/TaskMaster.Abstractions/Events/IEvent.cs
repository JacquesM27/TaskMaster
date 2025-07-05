namespace TaskMaster.Abstractions.Events;

public interface IEvent
{
}

public interface IDomainEvent : IEvent
{
    DateTime OccurredOn { get; }
    Guid AggregateId { get; }
}

public interface IIntegrationEvent : IEvent
{
    int Version { get; }
    string EventType { get; }
    DateTime PublishedAt { get; }
}