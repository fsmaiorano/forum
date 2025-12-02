using BuildingBlocks.Base;

namespace BuildingBlocks.Events;

public delegate void DomainEventCallback(object eventData);

public static class DomainEvents
{
    private static readonly Dictionary<string, List<DomainEventCallback>> HandlersMap = new();
    private static readonly List<Aggregate> MarkedAggregates = [];

    public static void MarkAggregateForDispatch(Aggregate aggregate)
    {
        var aggregateFound = FindMarkedAggregateById(aggregate.Id) != null;

        if (!aggregateFound)
        {
            MarkedAggregates.Add(aggregate);
        }
    }

    private static void DispatchAggregateEvents(Aggregate aggregate)
    {
        foreach (var domainEvent in aggregate.DomainEvents)
        {
            Dispatch(domainEvent);
        }
    }

    private static void RemoveAggregateFromMarkedDispatchList(Aggregate aggregate)
    {
        var index = MarkedAggregates.FindIndex(a => a.Equals(aggregate));
        if (index >= 0)
        {
            MarkedAggregates.RemoveAt(index);
        }
    }

    private static Aggregate? FindMarkedAggregateById(UniqueEntityId id)
    {
        return MarkedAggregates.Find(aggregate => aggregate.Id.Equals(id));
    }

    public static void DispatchEventsForAggregate(UniqueEntityId id)
    {
        var aggregate = FindMarkedAggregateById(id);

        if (aggregate == null) return;
        DispatchAggregateEvents(aggregate);
        aggregate.ClearEvents();
        RemoveAggregateFromMarkedDispatchList(aggregate);
    }

    public static void Register(DomainEventCallback callback, string eventClassName)
    {
        if (!HandlersMap.TryGetValue(eventClassName, out var value))
        {
            value = [];
            HandlersMap[eventClassName] = value;
        }

        value.Add(callback);
    }

    public static void ClearHandlers()
    {
        HandlersMap.Clear();
    }

    public static void ClearMarkedAggregates()
    {
        MarkedAggregates.Clear();
    }

    private static void Dispatch(IDomainEvent domainEvent)
    {
        var eventClassName = domainEvent.GetType().Name;

        if (!HandlersMap.TryGetValue(eventClassName, out var handlers)) return;
        foreach (var handler in handlers)
            handler(domainEvent);
    }
}