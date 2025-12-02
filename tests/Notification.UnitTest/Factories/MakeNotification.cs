using BuildingBlocks.Base;

namespace Forum.Notification.Factories;

public static class MakeNotification
{
    public static NotificationEntity Create(
        UniqueEntityId? recipientId = null,
        string? title = null,
        string? content = null,
        DateTime? readAt = null)
    {
        var faker = new Bogus.Faker();
        var notification = NotificationEntity.Create(
            recipientId ?? new UniqueEntityId(),
            title ?? faker.Lorem.Sentence(3),
            content ?? faker.Lorem.Paragraph());
        
        return notification;
    }

    public static SendNotificationCommand SendNotificationCommand(
        UniqueEntityId? recipientId = null,
        string? title = null,
        string? content = null)
    {
        var faker = new Bogus.Faker();
        return new SendNotificationCommand(
            RecipientId: recipientId ?? new UniqueEntityId(),
            Title: title ?? faker.Lorem.Sentence(3),
            Content: content ?? faker.Lorem.Paragraph()
        );
    }

    public static ReadNotificationCommand ReadNotificationCommand(
        UniqueEntityId? notificationId = null,
        UniqueEntityId? recipientId = null)
    {
        return new ReadNotificationCommand(
            NotificationId: notificationId ?? new UniqueEntityId(),
            RecipientId: recipientId ?? new UniqueEntityId()
        );
    }
}

