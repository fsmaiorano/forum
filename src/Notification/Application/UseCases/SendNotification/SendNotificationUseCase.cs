using BuildingBlocks.Base;
using BuildingBlocks.Logging;
using Notification.Domain.Entities;
using Notification.Domain.Repositories;

namespace Notification.Application.UseCases.SendNotification;

public record SendNotificationCommand(UniqueEntityId RecipientId, string Title, string Content);

public record SendNotificationResult();

public interface ISendNotificationUseCase
{
    Task<Result<SendNotificationResult>> SendNotificationUseCaseHandler(SendNotificationCommand command);
}

public sealed class SendNotificationUseCase(
    IAppLogger<SendNotificationUseCase> logger,
    INotificationRepository notificationRepository) : ISendNotificationUseCase
{
    public async Task<Result<SendNotificationResult>> SendNotificationUseCaseHandler(SendNotificationCommand command)
    {
        var notification = NotificationEntity.Create(
            command.RecipientId,
            command.Title,
            command.Content);

        await notificationRepository.Create(notification);

        logger.LogInformation(LogTypeEnum.Functional,
            $"Notification sent to user {notification.RecipientId} with ID: {notification.Id}");
        
        return Result<SendNotificationResult>.Success(new SendNotificationResult());
    }
}