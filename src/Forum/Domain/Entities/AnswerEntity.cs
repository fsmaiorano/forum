using Forum.BuildingBlocks.Base;
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
        string authorId,
        string questionId,
        string content,
        bool isClosed = false,
        WatchedList<AttachmentEntity>? attachments = null
    )
    {
        UniqueEntityId.Of(questionId);
        UniqueEntityId.Of(authorId);
        
        return new AnswerEntity
        {
            AuthorId = new UniqueEntityId(authorId),
            QuestionId = new UniqueEntityId(questionId),
            Content = content,
            IsClosed = isClosed,
            Attachments = attachments ?? AttachmentList.Create()
        };
    }
    
    public static string Excerpt(string content, int maxLength = 200)
    {
        if (string.IsNullOrEmpty(content))
            return string.Empty;

        return content.Length <= maxLength ? content : string.Concat(content.AsSpan(0, maxLength), "...");
    }
}