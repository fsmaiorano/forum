using Forum.Notification.Factories;

namespace Forum.Notification.Domain.Entities;

public class NotificationUnitTest
{
    [Fact]
    public void Create_ShouldCreateNotificationWithValidInputs()
    {
        var mock = MakeNotification.Create();

        var notification = NotificationEntity.Create(
            mock.RecipientId,
            mock.Title,
            mock.Content);

        Assert.Equal(mock.RecipientId.ToString(), notification.RecipientId.ToString());
        Assert.Equal(mock.Title, notification.Title);
        Assert.Equal(mock.Content, notification.Content);
        Assert.Null(notification.ReadAt);
    }

    [Fact]
    public void MarkAsRead_ShouldSetReadAtToCurrentTime()
    {
        var notification = MakeNotification.Create();
        var beforeMark = DateTime.UtcNow;

        notification.MarkAsRead();

        Assert.NotNull(notification.ReadAt);
        Assert.True(notification.ReadAt >= beforeMark);
        Assert.True(notification.ReadAt <= DateTime.UtcNow);
    }

    [Fact]
    public void Excerpt_ShouldReturnFullContentIfWithinMaxLength()
    {
        const string content = "Short content";

        var result = NotificationEntity.Excerpt(content, 50);

        Assert.Equal(content, result);
    }

    [Fact]
    public void Excerpt_ShouldReturnTruncatedContentWithEllipsis()
    {
        const string content = "This is a very long content that exceeds the maximum length and should be truncated with ellipsis at the end";

        var result = NotificationEntity.Excerpt(content, 20);

        Assert.Equal("This is a very long ...", result);
        Assert.Equal(23, result.Length); // 20 chars + "..."
    }

    [Fact]
    public void Excerpt_ShouldReturnEmptyStringForNullContent()
    {
        var result = NotificationEntity.Excerpt(null!);
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void Excerpt_ShouldReturnEmptyStringForEmptyContent()
    {
        var result = NotificationEntity.Excerpt(string.Empty);
        Assert.Equal(string.Empty, result);
    }
}

