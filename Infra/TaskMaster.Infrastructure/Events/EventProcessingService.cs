using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TaskMaster.Abstractions.Events;

namespace TaskMaster.Infrastructure.Events;

internal sealed class EventProcessingService(IServiceScopeFactory scopeFactory, EventQueue eventQueue) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var events = await eventQueue.DequeueAllAsync(stoppingToken);

            using var scope = scopeFactory.CreateScope();

            foreach (var @event in events)
            {
                var eventType = @event.GetType();
                var handlers = scope.ServiceProvider.GetServices(typeof(IEventHandler<>).MakeGenericType(eventType)).ToList();

                if (handlers.Count == 0)
                    continue;

                var tasks = handlers.Select(handler =>
                    ((Task)typeof(IEventHandler<>)
                        .MakeGenericType(eventType)
                        .GetMethod("HandleAsync")
                        .Invoke(handler, new object[] { @event, /*stoppingToken*/ }))
                );

                await Task.WhenAll(tasks);
            }
        }
    }
}