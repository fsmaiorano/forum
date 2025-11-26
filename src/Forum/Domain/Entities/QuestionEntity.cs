using Forum.BuildingBlocks.Base;

namespace Forum.Domain.Entities;

public sealed record QuestionEntity : Entity
{
    public UniqueEntityId AuthorId { get; private set; } = null!;
    public UniqueEntityId BestAnswerId { get; private set; } = null!;
    public string Title { get; private set; } = null!;
    public string Content { get; private set; } = null!;
    public Slug? Slug { get; private set; } = null;
    public WatchedList<AttachmentEntity> Attachments { get; private set; } = AttachmentList.Create();
    public bool IsOpen { get; private set; } = true;

    public static QuestionEntity Create(string authorId, string title, string content, string? slug = null)
    {
        var question = new QuestionEntity()
        {
            Id = new UniqueEntityId(),
            AuthorId = new UniqueEntityId(authorId),
            Title = title,
            Content = content,
            Slug = string.IsNullOrWhiteSpace(slug) ? Slug.Create(title) : new Slug(slug),
        };

        return question;
    }

    public static QuestionEntity Update(QuestionEntity questionEntity, string title, string content,
        string? slug = null)
    {
        questionEntity.Title = title;
        questionEntity.Content = content;
        questionEntity.Slug = string.IsNullOrWhiteSpace(slug) ? Slug.Create(title) : new Slug(slug);

        return questionEntity;
    }

    public static string Excerpt(string content, int maxLength = 200)
    {
        if (string.IsNullOrEmpty(content))
            return string.Empty;

        return content.Length <= maxLength ? content : string.Concat(content.AsSpan(0, maxLength), "...");
    }

    public void ChangeStatus()
    {
        IsOpen = !IsOpen;
        Touch();
    }
}