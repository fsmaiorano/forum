using BuildingBlocks.Base;
using BuildingBlocks.Exceptions;
using Forum.Notification.Base;
using Forum.Notification.Factories;
using Forum.Notification.Fixtures;

namespace Forum.Notification.Application.UseCases;

public class ReadNotificationUnitTest(TestFixture fixture) : BaseTest(fixture)
{
    [Fact]
    public async Task ReadNotificationUseCaseHandler_ShouldMarkNotificationAsRead()
    {
        var repository = new NotificationRepository(Context);
        var loggerMock = CreateLoggerMock<ReadNotificationUseCase>();
        var useCase = new ReadNotificationUseCase(loggerMock.Object, repository);

        var notification = MakeNotification.Create();
        await repository.Create(notification);

        var command = MakeNotification.ReadNotificationCommand(
            notification.Id,
            notification.RecipientId);

        var result = await useCase.ReadNotificationUseCaseHandler(command);

        var storedNotification = await repository.FindById(notification.Id);

        Assert.NotNull(storedNotification);
        Assert.True(result.IsSuccess);
        Assert.NotNull(storedNotification.ReadAt);
    }

    [Fact]
    public async Task ReadNotificationUseCaseHandler_ShouldThrowNotFoundException_WhenNotificationDoesNotExist()
    {
        var repository = new NotificationRepository(Context);
        var loggerMock = CreateLoggerMock<ReadNotificationUseCase>();
        var useCase = new ReadNotificationUseCase(loggerMock.Object, repository);

        var command = MakeNotification.ReadNotificationCommand();

        await Assert.ThrowsAsync<NotFoundException>(async () =>
            await useCase.ReadNotificationUseCaseHandler(command));
    }

    [Fact]
    public async Task ReadNotificationUseCaseHandler_ShouldThrowForbiddenException_WhenRecipientIdDoesNotMatch()
    {
        var repository = new NotificationRepository(Context);
        var loggerMock = CreateLoggerMock<ReadNotificationUseCase>();
        var useCase = new ReadNotificationUseCase(loggerMock.Object, repository);

        var notification = MakeNotification.Create();
        await repository.Create(notification);

        var command = MakeNotification.ReadNotificationCommand(
            notification.Id,
            new UniqueEntityId());

        await Assert.ThrowsAsync<ForbiddenException>(async () =>
            await useCase.ReadNotificationUseCaseHandler(command));
    }

    [Fact]
    public async Task ReadNotificationUseCaseHandler_ShouldNotUpdateReadAt_WhenAlreadyRead()
    {
        var repository = new NotificationRepository(Context);
        var loggerMock = CreateLoggerMock<ReadNotificationUseCase>();
        var useCase = new ReadNotificationUseCase(loggerMock.Object, repository);

        var notification = MakeNotification.Create();
        notification.MarkAsRead();
        var firstReadAt = notification.ReadAt;

        await repository.Create(notification);

        var command = MakeNotification.ReadNotificationCommand(
            notification.Id,
            notification.RecipientId);

        var result = await useCase.ReadNotificationUseCaseHandler(command);

        var storedNotification = await repository.FindById(notification.Id);

        Assert.NotNull(storedNotification);
        Assert.True(result.IsSuccess);
        Assert.NotNull(storedNotification.ReadAt);
        Assert.NotEqual(firstReadAt, storedNotification.ReadAt);
    }
}