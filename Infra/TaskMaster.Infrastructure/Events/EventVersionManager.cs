using Microsoft.Extensions.Logging;
using TaskMaster.Abstractions.Events;

namespace TaskMaster.Infrastructure.Events;

internal sealed class EventVersionManager(ILogger<EventVersionManager> logger)
{
    private readonly Dictionary<string, List<IEventMigration>> _migrations = new();

    public void RegisterMigration(IEventMigration migration)
    {
        if (!_migrations.ContainsKey(migration.EventType))
        {
            _migrations[migration.EventType] = new List<IEventMigration>();
        }

        _migrations[migration.EventType].Add(migration);
        logger.LogInformation("Registered migration for {EventType} from version {FromVersion} to {ToVersion}", 
            migration.EventType, migration.FromVersion, migration.ToVersion);
    }

    public IIntegrationEvent MigrateEvent(IIntegrationEvent @event, int targetVersion)
    {
        if (@event.Version == targetVersion)
        {
            return @event;
        }

        if (!_migrations.ContainsKey(@event.EventType))
        {
            logger.LogWarning("No migrations found for event type {EventType}", @event.EventType);
            return @event;
        }

        var currentEvent = @event;
        var currentVersion = @event.Version;

        while (currentVersion < targetVersion)
        {
            var migration = _migrations[@event.EventType]
                .FirstOrDefault(m => m.FromVersion == currentVersion);

            if (migration == null)
            {
                logger.LogWarning("No migration found from version {CurrentVersion} for event type {EventType}", 
                    currentVersion, @event.EventType);
                break;
            }

            currentEvent = migration.Migrate(currentEvent);
            currentVersion = migration.ToVersion;

            logger.LogDebug("Migrated event {EventType} from version {FromVersion} to {ToVersion}", 
                @event.EventType, migration.FromVersion, migration.ToVersion);
        }

        return currentEvent;
    }
}