using BuildingBlocks.Base;

namespace BuildingBlocks.Events;

public abstract record Aggregate : Entity
{
    private readonly List<IDomainEvent> _domainEvents = [];

    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
        Events.DomainEvents.MarkAggregateForDispatch(this);
    }

    public void ClearEvents()
    {
        _domainEvents.Clear();
    }
}