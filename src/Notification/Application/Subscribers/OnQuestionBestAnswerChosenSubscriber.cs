using BuildingBlocks.Base;
using BuildingBlocks.Logging;
using BuildingBlocks.Messaging;
using BuildingBlocks.Messaging.IntegrationEvents;
using BuildingBlocks.Messaging.IntegrationEvents.Interfaces;
using Notification.Application.UseCases.SendNotification;

namespace Notification.Application.Subscribers;

public class OnQuestionBestAnswerChosenSubscriber(
    IAppLogger<OnQuestionBestAnswerChosenSubscriber> logger,
    ISendNotificationUseCase sendNotificationUseCase)
    : IIntegrationEventHandler<QuestionBestAnswerChosenIntegrationEvent>
{
    public async Task HandleAsync(QuestionBestAnswerChosenIntegrationEvent @event, CancellationToken cancellationToken = default)
    {
        logger.LogInformation(
            LogTypeEnum.Functional,
            "Received QuestionBestAnswerChosenIntegrationEvent - QuestionId: {QuestionId}, BestAnswerId: {BestAnswerId}",
            @event.QuestionId,
            @event.BestAnswerId
        );

        try
        {
            var recipientId = UniqueEntityId.Of(@event.AnswerAuthorId);

            var command = new SendNotificationCommand(
                RecipientId: recipientId,
                Title: "Your Answer Was Chosen as Best Answer!",
                Content: $"Congratulations! Your answer to '{@event.QuestionTitle}' was selected as the best answer."
            );

            await sendNotificationUseCase.SendNotificationUseCaseHandler(command);

            logger.LogInformation(
                LogTypeEnum.Functional,
                "Notification sent for best answer chosen - AnswerId: {AnswerId}, RecipientId: {RecipientId}",
                @event.BestAnswerId,
                @event.AnswerAuthorId
            );
        }
        catch (Exception ex)
        {
            logger.LogError(
                LogTypeEnum.Exception,
                ex,
                "Error handling QuestionBestAnswerChosenIntegrationEvent - EventId: {EventId}",
                @event.EventId
            );
        }
    }
}

