using BuildingBlocks.Base;
using BuildingBlocks.Events;

namespace Forum.Domain.Entities;

public sealed record QuestionEntity : Aggregate
{
    public UniqueEntityId AuthorId { get; private set; } = null!;
    public UniqueEntityId BestAnswerId { get; private set; } = null!;
    public string Title { get; private set; } = null!;
    public string Content { get; private set; } = null!;
    public Slug? Slug { get; private set; } = null;
    public WatchedList<AttachmentEntity> Attachments { get; private set; } = null!;
    public bool IsOpen { get; private set; } = true;

    public static QuestionEntity Create(UniqueEntityId authorId, string title, string content, string? slug = null,
        WatchedList<AttachmentEntity>? attachments = null)
    {
        var question = new QuestionEntity()
        {
            Id = new UniqueEntityId(),
            AuthorId = authorId,
            Title = title,
            Content = content,
            Slug = string.IsNullOrWhiteSpace(slug) ? Slug.Create(title) : new Slug(slug),
            Attachments = attachments ?? AttachmentList.Create()
        };

        return question;
    }

    public static QuestionEntity Update(QuestionEntity questionEntity, string title, string content,
        string? slug = null, WatchedList<AttachmentEntity>? attachments = null)
    {
        questionEntity.Title = title;
        questionEntity.Content = content;
        questionEntity.Slug = string.IsNullOrWhiteSpace(slug) ? Slug.Create(title) : new Slug(slug);

        if (attachments != null && attachments.GetItems().Count != 0 && questionEntity.Attachments.GetItems().Count > 0)
            questionEntity.Attachments.Update(attachments.GetItems());

        return questionEntity;
    }

    public static QuestionEntity SelectBestAnswer(QuestionEntity questionEntity, UniqueEntityId answerId)
    {
        questionEntity.BestAnswerId = answerId;
        questionEntity.Touch();
        
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