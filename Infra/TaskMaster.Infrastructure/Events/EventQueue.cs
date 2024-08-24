using System.Threading.Channels;
using TaskMaster.Abstractions.Events;

namespace TaskMaster.Infrastructure.Events;

internal sealed class EventQueue
{
    private readonly Channel<IEvent> _events = Channel.CreateUnbounded<IEvent>();

    public async Task QueueEventAsync<TEvent>(TEvent @event) where TEvent : class, IEvent
        => await _events.Writer.WriteAsync(@event);
    
    public async Task<IEnumerable<object>> DequeueAllAsync(CancellationToken cancellationToken)
    {
        var events = new List<object>();
        while (await _events.Reader.WaitToReadAsync(cancellationToken))
        {
            while (_events.Reader.TryRead(out var @event))
            {
                events.Add(@event);
            }
        }
        return events;
    }
}