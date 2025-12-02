using BuildingBlocks.Events;
using BuildingBlocks.Logging;
using BuildingBlocks.Messaging;
using BuildingBlocks.Messaging.Events;
using Forum.Domain.Events;
using Forum.Domain.Repositories;

namespace Forum.Application.EventHandlers;

public class OnAnswerCreated(
    IAppLogger<OnAnswerCreated> logger,
    IEventBus eventBus,
    IAnswerRepository answerRepository) : IEventHandler
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

        // Publish integration event for notification service
        Task.Run(async () =>
        {
            var answer = await answerRepository.FindById(answerCreatedEvent.GetAggregateId());
            if (answer is null) return;

            var integrationEvent = new AnswerCreatedIntegrationEvent
            {
                AnswerId = answer.Id.ToString(),
                QuestionId = answer.QuestionId.ToString(),
                AuthorId = answer.AuthorId.ToString(),
                Content = answer.Content
            };

            await eventBus.PublishAsync(integrationEvent);
        });
    }
}