using BuildingBlocks.Events;
using BuildingBlocks.Logging;
using Forum.Domain.Events;

namespace Forum.Application.EventHandlers;

public class OnAnswerCreated(IAppLogger<OnAnswerCreated> logger) : IEventHandler
{
    public void SetupSubscriptions()
    {
        DomainEvents.Register(Handle, nameof(AnswerCreatedEvent));
    }

    private void Handle(object eventData)
    {
        if (eventData is not AnswerCreatedEvent answerCreatedEvent) return;

        logger.LogInformation(
            LogType.Functional,
            "Answer created event handled - AggregateId: {AggregateId}, OccurredAt: {OccurredAt}",
            answerCreatedEvent.GetAggregateId().ToString(),
            answerCreatedEvent.OccurredAt
        );
    }
}