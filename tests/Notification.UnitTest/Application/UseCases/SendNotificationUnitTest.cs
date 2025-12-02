using BuildingBlocks.Base;
using Forum.Notification.Base;
using Forum.Notification.Factories;
using Forum.Notification.Fixtures;

namespace Forum.Notification.Application.UseCases;

public class SendNotificationUnitTest(TestFixture fixture) : BaseTest(fixture)
{
    [Fact]
    public async Task SendNotificationUseCaseHandler_ShouldCreateNotification()
    {
        var repository = new NotificationRepository(Context);
        var loggerMock = CreateLoggerMock<SendNotificationUseCase>();
        var useCase = new SendNotificationUseCase(loggerMock.Object, repository);

        var command = MakeNotification.SendNotificationCommand();
        var result = await useCase.SendNotificationUseCaseHandler(command);

        var storedNotification = await Context.Notification
            .FirstOrDefaultAsync(n => n.RecipientId == command.RecipientId);

        Assert.NotNull(storedNotification);
        Assert.True(result.IsSuccess);
        Assert.Equal(command.Title, storedNotification.Title);
        Assert.Equal(command.Content, storedNotification.Content);
        Assert.Equal(command.RecipientId, storedNotification.RecipientId);
        Assert.Null(storedNotification.ReadAt);
    }

    [Fact]
    public async Task SendNotificationUseCaseHandler_ShouldCreateMultipleNotifications()
    {
        var repository = new NotificationRepository(Context);
        var loggerMock = CreateLoggerMock<SendNotificationUseCase>();
        var useCase = new SendNotificationUseCase(loggerMock.Object, repository);

        var recipientId = new UniqueEntityId();
        var command1 = MakeNotification.SendNotificationCommand(recipientId);
        var command2 = MakeNotification.SendNotificationCommand(recipientId);

        var result1 = await useCase.SendNotificationUseCaseHandler(command1);
        var result2 = await useCase.SendNotificationUseCaseHandler(command2);

        var storedNotifications = Context.Notification
            .Where(n => n.RecipientId == recipientId)
            .ToList();

        Assert.True(result1.IsSuccess);
        Assert.True(result2.IsSuccess);
        Assert.Equal(2, storedNotifications.Count);
    }
}

