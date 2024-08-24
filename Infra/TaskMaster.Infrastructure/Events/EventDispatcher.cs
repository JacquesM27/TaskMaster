using TaskMaster.Abstractions.Events;

namespace TaskMaster.Infrastructure.Events;

internal sealed class EventDispatcher(EventQueue eventQueue) : IEventDispatcher
{
    public Task PublishAsync<TEvent>(TEvent @event) where TEvent : class, IEvent 
        => eventQueue.QueueEventAsync(@event);
}