namespace TaskMaster.Abstractions.Events;

public interface IEventMigration
{
    string EventType { get; }
    int FromVersion { get; }
    int ToVersion { get; }
    IIntegrationEvent Migrate(IIntegrationEvent @event);
}

public interface IEventMigration<T> : IEventMigration where T : IIntegrationEvent
{
    T Migrate(T @event);
}