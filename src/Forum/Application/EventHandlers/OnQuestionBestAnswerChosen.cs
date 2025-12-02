using BuildingBlocks.Events;
using BuildingBlocks.Logging;
using Forum.Domain.Events;

namespace Forum.Application.EventHandlers;

public class OnQuestionBestAnswerChosen(IAppLogger<OnQuestionBestAnswerChosen> logger) : IEventHandler
{
    public void SetupSubscriptions()
    {
        DomainEvents.Register(Handle, nameof(QuestionBestAnswerChosenEvent));
    }

    private void Handle(object eventData)
    {
        if (eventData is not QuestionBestAnswerChosenEvent bestAnswerEvent) return;

        logger.LogInformation(
            LogType.Functional,
            "Question best answer chosen event handled - QuestionId: {QuestionId}, BestAnswerId: {BestAnswerId}, OccurredAt: {OccurredAt}",
            bestAnswerEvent.GetAggregateId().ToString(),
            bestAnswerEvent.BestAnswerId.ToString(),
            bestAnswerEvent.OccurredAt
        );
    }
}