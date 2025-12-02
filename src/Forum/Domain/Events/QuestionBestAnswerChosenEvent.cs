using BuildingBlocks.Base;
using BuildingBlocks.Messaging.DomainEvents;

namespace Forum.Domain.Events;

public class QuestionBestAnswerChosenEvent(QuestionEntity question, UniqueEntityId bestAnswerId) : IDomainEvent
{
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
    public UniqueEntityId BestAnswerId { get; } = bestAnswerId;
    public QuestionEntity Question { get; } = question;

    public UniqueEntityId GetAggregateId()
    {
        return Question.Id;
    }
}