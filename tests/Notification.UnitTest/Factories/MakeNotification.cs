using BuildingBlocks.Base;
using Notification.Endpoints.Notification;

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

    public static SendNotificationRequest SendNotificationRequest(
        string? recipientId = null,
        string? title = null,
        string? content = null)
    {
        var faker = new Bogus.Faker();
        return new SendNotificationRequest(
            RecipientId: recipientId ?? faker.Random.Uuid().ToString(),
            Title: title ?? faker.Lorem.Sentence(20),
            Content: content ?? faker.Lorem.Paragraph()
        );
    }

    public static ReadNotificationRequest ReadNotificationRequest(string? notificationId, string? recipientId)
    {
        var faker = new Bogus.Faker();
        return new ReadNotificationRequest(notificationId ?? faker.Random.Uuid().ToString(),
            recipientId ?? faker.Random.Uuid().ToString());
    }
}