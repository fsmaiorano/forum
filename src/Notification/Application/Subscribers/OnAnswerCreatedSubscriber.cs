using BuildingBlocks.Base;
using BuildingBlocks.Logging;
using BuildingBlocks.Messaging.IntegrationEvents;
using BuildingBlocks.Messaging.IntegrationEvents.Interfaces;
using Notification.Application.UseCases.SendNotification;

namespace Notification.Application.Subscribers;

public class OnAnswerCreatedSubscriber(
    IAppLogger<OnAnswerCreatedSubscriber> logger,
    ISendNotificationUseCase sendNotificationUseCase)
    : IIntegrationEventHandler<AnswerCreatedIntegrationEvent>
{
    public async Task HandleAsync(AnswerCreatedIntegrationEvent @event, CancellationToken cancellationToken = default)
    {
        logger.LogInformation(
            LogTypeEnum.Functional,
            "Received AnswerCreatedIntegrationEvent - AnswerId: {AnswerId}, QuestionId: {QuestionId}",
            @event.AnswerId,
            @event.QuestionId
        );

        try
        {
            var recipientId = UniqueEntityId.Of(@event.QuestionId);

            var command = new SendNotificationCommand(
                RecipientId: recipientId,
                Title: "New Answer to Your Question",
                Content: $"Someone answered your question. Answer preview: {GetExcerpt(@event.Content)}"
            );

            await sendNotificationUseCase.SendNotificationUseCaseHandler(command);

            logger.LogInformation(
                LogTypeEnum.Functional,
                "Notification sent for new answer - AnswerId: {AnswerId}",
                @event.AnswerId
            );
        }
        catch (Exception ex)
        {
            logger.LogError(
                LogTypeEnum.Exception,
                ex,
                "Error handling AnswerCreatedIntegrationEvent - EventId: {EventId}",
                @event.EventId
            );
        }
    }

    private static string GetExcerpt(string content, int maxLength = 100)
    {
        if (string.IsNullOrEmpty(content))
            return string.Empty;

        return content.Length <= maxLength 
            ? content 
            : string.Concat(content.AsSpan(0, maxLength), "...");
    }
}
