using Microsoft.Extensions.Logging;
using System.Text.Json;
using TaskMaster.Abstractions.Events;
using TaskMaster.Abstractions.Outbox;

namespace TaskMaster.Infrastructure.Events;

internal sealed class IntegrationEventBus(
    IOutboxRepository outboxRepository,
    IIntegrationEventStore eventStore,
    ILogger<IntegrationEventBus> logger) : IIntegrationEventBus
{
    public async Task PublishAsync<T>(T @event, CancellationToken cancellationToken = default) where T : IIntegrationEvent
    {
        try
        {
            var outboxMessage = new OutboxMessage
            {
                Id = Guid.NewGuid(),
                EventType = @event.EventType,
                EventData = JsonSerializer.Serialize(@event),
                Source = typeof(T).Assembly.GetName().Name ?? "Unknown",
                CreatedAt = DateTime.UtcNow
            };

            await outboxRepository.AddAsync(outboxMessage, cancellationToken);
            await eventStore.SaveAsync(@event, cancellationToken);

            logger.LogInformation("Integration event {EventType} published successfully", @event.EventType);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to publish integration event {EventType}", @event.EventType);
            throw;
        }
    }

    public async Task PublishBatchAsync<T>(IEnumerable<T> events, CancellationToken cancellationToken = default) where T : IIntegrationEvent
    {
        var eventList = events.ToList();
        logger.LogInformation("Publishing batch of {Count} integration events", eventList.Count);

        foreach (var @event in eventList)
        {
            await PublishAsync(@event, cancellationToken);
        }
    }
}