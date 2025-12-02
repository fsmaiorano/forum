using BuildingBlocks.Base;
using BuildingBlocks.Events;

namespace Forum.Domain.Events;

public class QuestionBestAnswerChosenEvent(QuestionEntity question, UniqueEntityId bestAnswerId) : IDomainEvent
{
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
    public UniqueEntityId BestAnswerId { get; } = bestAnswerId;
    private QuestionEntity Question { get; } = question;

    public UniqueEntityId GetAggregateId()
    {
        return Question.Id;
    }
}