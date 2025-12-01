using BuildingBlocks.Base;
using Forum.Domain.Enums;

namespace Forum.Domain.Entities;

public sealed record AnswerEntity : Entity
{
    public UniqueEntityId AuthorId { get; private set; } = null!;
    public UniqueEntityId QuestionId { get; private set; } = null!;
    public string Content { get; private set; } = null!;
    public bool IsClosed { get; private set; }
    public WatchedList<AttachmentEntity> Attachments { get; private set; } = null!;


    public static AnswerEntity Create(
        UniqueEntityId authorId,
        UniqueEntityId questionId,
        string content,
        bool isClosed = false,
        WatchedList<AttachmentEntity>? attachments = null
    )
    {
        UniqueEntityId.Of(questionId);
        UniqueEntityId.Of(authorId);

        return new AnswerEntity
        {
            Id = new UniqueEntityId(),
            AuthorId = authorId,
            QuestionId = questionId,
            Content = content,
            IsClosed = isClosed,
            Attachments = attachments ?? AttachmentList.Create()
        };
    }

    public static AnswerEntity Update(AnswerEntity answerEntity, string content, bool isClosed = false,
        WatchedList<AttachmentEntity>? attachments = null)
    {
        answerEntity.Content = content;
        answerEntity.IsClosed = isClosed;
        
        if (attachments != null && attachments.GetItems().Count != 0 && answerEntity.Attachments.GetItems().Count > 0)
            answerEntity.Attachments.Update(attachments.GetItems());
        
        return answerEntity;
    }

    public static string Excerpt(string content, int maxLength = 200)
    {
        if (string.IsNullOrEmpty(content))
            return string.Empty;

        return content.Length <= maxLength ? content : string.Concat(content.AsSpan(0, maxLength), "...");
    }
}