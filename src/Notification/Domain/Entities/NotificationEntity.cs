using BuildingBlocks.Base;

namespace Notification.Domain.Entities;

public sealed record NotificationEntity : Entity
{
    public string Title { get; private set; } = string.Empty;
    public string Content { get; private set; } = string.Empty;
    public UniqueEntityId RecipientId { get; private set; } = null!;
    public DateTime? ReadAt { get; private set; } = null;

    public static NotificationEntity Create(
        UniqueEntityId recipientId,
        string title,
        string content)
    {
        UniqueEntityId.Of(recipientId);

        return new NotificationEntity()
        {
            Id = new UniqueEntityId(),
            RecipientId = recipientId,
            Title = title,
            Content = content
        };
    }

    public void MarkAsRead()
    {
        ReadAt = DateTime.UtcNow;
    }

    public static string Excerpt(string content, int maxLength = 200)
    {
        if (string.IsNullOrEmpty(content))
            return string.Empty;

        return content.Length <= maxLength ? content : string.Concat(content.AsSpan(0, maxLength), "...");
    }
}