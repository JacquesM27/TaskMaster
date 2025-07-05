using Microsoft.EntityFrameworkCore;
using TaskMaster.Abstractions.Outbox;
using TaskMaster.Infrastructure.DAL;

namespace TaskMaster.Infrastructure.Outbox;

internal sealed class OutboxRepository(TaskMasterDbContext context) : IOutboxRepository
{
    public async Task AddAsync(OutboxMessage message, CancellationToken cancellationToken = default)
    {
        await context.OutboxMessages.AddAsync(message, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<OutboxMessage>> GetUnprocessedAsync(int batchSize = 100, CancellationToken cancellationToken = default)
    {
        return await context.OutboxMessages
            .Where(m => m.ProcessedAt == null)
            .OrderBy(m => m.CreatedAt)
            .Take(batchSize)
            .ToListAsync(cancellationToken);
    }

    public async Task MarkAsProcessedAsync(Guid messageId, CancellationToken cancellationToken = default)
    {
        var message = await context.OutboxMessages.FindAsync(messageId, cancellationToken);
        if (message != null)
        {
            message.ProcessedAt = DateTime.UtcNow;
            await context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task IncrementRetryCountAsync(Guid messageId, CancellationToken cancellationToken = default)
    {
        var message = await context.OutboxMessages.FindAsync(messageId, cancellationToken);
        if (message != null)
        {
            message.RetryCount++;
            await context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<IEnumerable<OutboxMessage>> GetFailedMessagesAsync(int maxRetries = 3, CancellationToken cancellationToken = default)
    {
        return await context.OutboxMessages
            .Where(m => m.ProcessedAt == null && m.RetryCount >= maxRetries)
            .OrderBy(m => m.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}