using BuildingBlocks.Events;
using BuildingBlocks.Logging;
using BuildingBlocks.Messaging;
using BuildingBlocks.Messaging.Events;
using Forum.Domain.Events;
using Forum.Domain.Repositories;

namespace Forum.Application.EventHandlers;

public class OnQuestionBestAnswerChosen(
    IAppLogger<OnQuestionBestAnswerChosen> logger,
    IEventBus eventBus,
    IAnswerRepository answerRepository) : IEventHandler
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

        // Publish integration event for notification service
        Task.Run(async () =>
        {
            var answer = await answerRepository.FindById(bestAnswerEvent.BestAnswerId);
            if (answer is null) return;

            var integrationEvent = new QuestionBestAnswerChosenIntegrationEvent
            {
                QuestionId = bestAnswerEvent.Question.Id.ToString(),
                BestAnswerId = bestAnswerEvent.BestAnswerId.ToString(),
                QuestionAuthorId = bestAnswerEvent.Question.AuthorId.ToString(),
                AnswerAuthorId = answer.AuthorId.ToString(),
                QuestionTitle = bestAnswerEvent.Question.Title
            };

            await eventBus.PublishAsync(integrationEvent);
        });
    }
}