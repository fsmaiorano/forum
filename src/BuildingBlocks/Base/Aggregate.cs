using BuildingBlocks.Messaging.DomainEvents;

namespace BuildingBlocks.Base;

/// <summary>
/// Base class for aggregates that support domain events
/// </summary>
public abstract record Aggregate : Entity
{
    private readonly List<IDomainEvent> _domainEvents = [];

    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void ClearEvents()
    {
        _domainEvents.Clear();
    }
}