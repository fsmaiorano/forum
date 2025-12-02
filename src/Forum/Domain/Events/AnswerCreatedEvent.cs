using BuildingBlocks.Base;
using BuildingBlocks.Messaging.DomainEvents;

namespace Forum.Domain.Events;

public class AnswerCreatedEvent(AnswerEntity answerEntity) : IDomainEvent
{
    public DateTime OccurredAt { get; } = DateTime.UtcNow;

    public UniqueEntityId GetAggregateId()
    {
        return answerEntity.Id;
    }
}