using BuildingBlocks.Logging;
using BuildingBlocks.Messaging;
using BuildingBlocks.Messaging.DomainEvents;
using BuildingBlocks.Messaging.DomainEvents.Interfaces;
using BuildingBlocks.Messaging.IntegrationEvents;
using Forum.Domain.Events;
using Forum.Domain.Repositories;

namespace Forum.Application.EventHandlers;

/// <summary>
/// Domain event handler that publishes integration event when a best answer is chosen
/// </summary>
public class OnQuestionBestAnswerChosenEventHandler(
    IAppLogger<OnQuestionBestAnswerChosenEventHandler> logger,
    IEventBus eventBus,
    IAnswerRepository answerRepository) : IDomainEventHandler<QuestionBestAnswerChosenEvent>
{
    public async Task HandleAsync(QuestionBestAnswerChosenEvent @event, CancellationToken cancellationToken = default)
    {
        logger.LogInformation(
            LogType.Functional,
            "Handling QuestionBestAnswerChosenEvent - QuestionId: {QuestionId}, BestAnswerId: {BestAnswerId}, OccurredAt: {OccurredAt}",
            @event.GetAggregateId().ToString(),
            @event.BestAnswerId.ToString(),
            @event.OccurredAt
        );

        try
        {
            var answer = await answerRepository.FindById(@event.BestAnswerId);
            if (answer is null)
            {
                logger.LogWarning(
                    LogType.Functional,
                    "Answer not found for QuestionBestAnswerChosenEvent - AnswerId: {AnswerId}",
                    @event.BestAnswerId.ToString()
                );
                return;
            }

            var integrationEvent = new QuestionBestAnswerChosenIntegrationEvent
            {
                QuestionId = @event.Question.Id.ToString(),
                BestAnswerId = @event.BestAnswerId.ToString(),
                QuestionAuthorId = @event.Question.AuthorId.ToString(),
                AnswerAuthorId = answer.AuthorId.ToString(),
                QuestionTitle = @event.Question.Title
            };

            await eventBus.PublishAsync(integrationEvent, cancellationToken);

            logger.LogInformation(
                LogType.Functional,
                "Published QuestionBestAnswerChosenIntegrationEvent - QuestionId: {QuestionId}, BestAnswerId: {BestAnswerId}",
                @event.Question.Id.ToString(),
                @event.BestAnswerId.ToString()
            );
        }
        catch (Exception ex)
        {
            logger.LogError(
                LogType.Exception,
                ex,
                "Error handling QuestionBestAnswerChosenEvent - QuestionId: {QuestionId}",
                @event.GetAggregateId().ToString()
            );
            throw;
        }
    }
}