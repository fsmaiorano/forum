using BuildingBlocks.Messaging.DomainEvents;

namespace Forum.UnitTests.Fixtures;

/// <summary>
/// Test implementation of IDomainEventDispatcher that collects events without dispatching
/// This allows tests to verify events were raised without triggering side effects
/// </summary>
public class TestDomainEventDispatcher : IDomainEventDispatcher
{
    private readonly List<IDomainEvent> _dispatchedEvents = [];

    public IReadOnlyList<IDomainEvent> DispatchedEvents => _dispatchedEvents.AsReadOnly();

    public Task DispatchAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        _dispatchedEvents.Add(domainEvent);
        return Task.CompletedTask;
    }

    public Task DispatchAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default)
    {
        _dispatchedEvents.AddRange(domainEvents);
        return Task.CompletedTask;
    }

    public void Clear()
    {
        _dispatchedEvents.Clear();
    }

    public bool HasEvent<TEvent>() where TEvent : IDomainEvent
    {
        return _dispatchedEvents.OfType<TEvent>().Any();
    }

    public TEvent? GetEvent<TEvent>() where TEvent : IDomainEvent
    {
        return _dispatchedEvents.OfType<TEvent>().FirstOrDefault();
    }

    public IEnumerable<TEvent> GetEvents<TEvent>() where TEvent : IDomainEvent
    {
        return _dispatchedEvents.OfType<TEvent>();
    }
}