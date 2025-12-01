using BuildingBlocks.Base;

namespace BuildingBlocks.Events;

public interface IDomainEvent
{
    DateTime OccurredAt { get; }
    UniqueEntityId GetAggregateId();
}