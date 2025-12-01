using BuildingBlocks.Base;
using BuildingBlocks.Events;

namespace Forum.Domain.Events;

public class QuestionBestAnswerChosenEvent(QuestionEntity Question, UniqueEntityId BestAnswerId) : IDomainEvent
{
    public DateTime OccurredAt { get; }
    
    public UniqueEntityId GetAggregateId()
    {
        return Question.Id;
    }
}