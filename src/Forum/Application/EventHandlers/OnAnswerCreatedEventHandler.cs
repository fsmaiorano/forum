using BuildingBlocks.Logging;
using BuildingBlocks.Messaging;
using BuildingBlocks.Messaging.DomainEvents;
using BuildingBlocks.Messaging.DomainEvents.Interfaces;
using BuildingBlocks.Messaging.IntegrationEvents;
using Forum.Domain.Events;
using Forum.Domain.Repositories;

namespace Forum.Application.EventHandlers;

/// <summary>
/// Domain event handler that publishes integration event when an answer is created
/// </summary>
public class OnAnswerCreatedEventHandler(
    IAppLogger<OnAnswerCreatedEventHandler> logger,
    IEventBus eventBus,
    IAnswerRepository answerRepository) : IDomainEventHandler<AnswerCreatedEvent>
{
    public async Task HandleAsync(AnswerCreatedEvent @event, CancellationToken cancellationToken = default)
    {
        logger.LogInformation(
            LogType.Functional,
            "Handling AnswerCreatedEvent - AggregateId: {AggregateId}, OccurredAt: {OccurredAt}",
            @event.GetAggregateId().ToString(),
            @event.OccurredAt
        );

        try
        {
            var answer = await answerRepository.FindById(@event.GetAggregateId());
            if (answer is null)
            {
                logger.LogWarning(
                    LogType.Functional,
                    "Answer not found for AnswerCreatedEvent - AnswerId: {AnswerId}",
                    @event.GetAggregateId().ToString()
                );
                return;
            }

            var integrationEvent = new AnswerCreatedIntegrationEvent
            {
                AnswerId = answer.Id.ToString(),
                QuestionId = answer.QuestionId.ToString(),
                AuthorId = answer.AuthorId.ToString(),
                Content = answer.Content
            };

            await eventBus.PublishAsync(integrationEvent, cancellationToken);

            logger.LogInformation(
                LogType.Functional,
                "Published AnswerCreatedIntegrationEvent - AnswerId: {AnswerId}",
                answer.Id.ToString()
            );
        }
        catch (Exception ex)
        {
            logger.LogError(
                LogType.Exception,
                ex,
                "Error handling AnswerCreatedEvent - AggregateId: {AggregateId}",
                @event.GetAggregateId().ToString()
            );
            throw;
        }
    }
}