using BuildingBlocks.Base;
using BuildingBlocks.Exceptions;
using BuildingBlocks.Logging;
using Notification.Domain.Repositories;

namespace Notification.Application.UseCases.ReadNotification;

public record ReadNotificationCommand(UniqueEntityId NotificationId, UniqueEntityId RecipientId);

public record ReadNotificationResult();

public interface IReadNotificationUseCase
{
    Task<Result<ReadNotificationResult>> ReadNotificationUseCaseHandler(ReadNotificationCommand command);
}

public sealed class ReadNotificationUseCase(
    IAppLogger<ReadNotificationUseCase> logger,
    INotificationRepository notificationRepository) : IReadNotificationUseCase
{
    public async Task<Result<ReadNotificationResult>> ReadNotificationUseCaseHandler(ReadNotificationCommand command)
    {
        var notification = await
            notificationRepository.FindById(command.NotificationId);

        if (notification is null)
            throw new NotFoundException($"Notification with ID {command.NotificationId} not found.");

        if (!Equals(notification.RecipientId, command.RecipientId))
            throw new ForbiddenException("You are not allowed to read this notification.");

        notification.MarkAsRead();

        await notificationRepository.Update(notification);

        logger.LogInformation(LogType.Functional,
            $"Notification read for user {notification.RecipientId} with ID: {notification.Id}");

        return Result<ReadNotificationResult>.Success(new ReadNotificationResult());
    }
}