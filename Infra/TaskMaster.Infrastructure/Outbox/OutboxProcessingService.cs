using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using TaskMaster.Abstractions.Events;
using TaskMaster.Abstractions.Outbox;

namespace TaskMaster.Infrastructure.Outbox;

internal sealed class OutboxProcessingService(
    IServiceScopeFactory scopeFactory,
    ILogger<OutboxProcessingService> logger) : BackgroundService
{
    private static readonly TimeSpan ProcessingInterval = TimeSpan.FromSeconds(10);
    private static readonly int BatchSize = 100;
    private static readonly int MaxRetries = 3;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Outbox processing service started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessOutboxMessagesAsync(stoppingToken);
                await Task.Delay(ProcessingInterval, stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing outbox messages");
                await Task.Delay(ProcessingInterval, stoppingToken);
            }
        }
    }

    private async Task ProcessOutboxMessagesAsync(CancellationToken cancellationToken)
    {
        using var scope = scopeFactory.CreateScope();
        var outboxRepository = scope.ServiceProvider.GetRequiredService<IOutboxRepository>();
        var eventDispatcher = scope.ServiceProvider.GetRequiredService<IEventDispatcher>();

        var messages = await outboxRepository.GetUnprocessedAsync(BatchSize, cancellationToken);
        var messageList = messages.ToList();

        if (messageList.Count == 0)
        {
            return;
        }

        logger.LogInformation("Processing {Count} outbox messages", messageList.Count);

        foreach (var message in messageList)
        {
            try
            {
                var @event = DeserializeEvent(message);
                if (@event != null)
                {
                    await eventDispatcher.PublishAsync(@event);
                    await outboxRepository.MarkAsProcessedAsync(message.Id, cancellationToken);
                    logger.LogDebug("Processed outbox message {MessageId} of type {EventType}", 
                        message.Id, message.EventType);
                }
                else
                {
                    logger.LogWarning("Failed to deserialize outbox message {MessageId} of type {EventType}", 
                        message.Id, message.EventType);
                    await outboxRepository.IncrementRetryCountAsync(message.Id, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing outbox message {MessageId} of type {EventType}", 
                    message.Id, message.EventType);
                await outboxRepository.IncrementRetryCountAsync(message.Id, cancellationToken);
            }
        }

        await ProcessFailedMessagesAsync(outboxRepository, cancellationToken);
    }

    private async Task ProcessFailedMessagesAsync(IOutboxRepository outboxRepository, CancellationToken cancellationToken)
    {
        var failedMessages = await outboxRepository.GetFailedMessagesAsync(MaxRetries, cancellationToken);
        var failedList = failedMessages.ToList();

        if (failedList.Count > 0)
        {
            logger.LogWarning("Found {Count} failed outbox messages that exceeded max retries", failedList.Count);
        }
    }

    private static IEvent? DeserializeEvent(OutboxMessage message)
    {
        try
        {
            var eventType = Type.GetType(message.EventType);
            if (eventType == null) return null;

            return JsonSerializer.Deserialize(message.EventData, eventType) as IEvent;
        }
        catch
        {
            return null;
        }
    }
}