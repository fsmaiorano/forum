using BuildingBlocks.Base;

namespace BuildingBlocks.Messaging.DomainEvents;

public interface IDomainEvent
{
    DateTime OccurredAt { get; }
    UniqueEntityId GetAggregateId();
}