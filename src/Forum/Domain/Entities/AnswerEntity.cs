using Forum.BuildingBlocks.Base;
using Forum.Domain.Enums;

namespace Forum.Domain.Entities;

public sealed record AnswerEntity : Entity
{
    public UniqueEntityId AuthorId { get; private set; } = null!;
    public UniqueEntityId QuestionId { get; private set; } = null!;
    public string Content { get; private set; } = null!;
    public bool IsClosed { get; private set; }
    public IEnumerable<AttachmentEntity> Attachments { get; private set; } = [];


    public static AnswerEntity Create(
        string authorId,
        string questionId,
        string content,
        bool isClosed = false,
        List<AttachmentEntity>? attachments = null
    )
    {
        return new AnswerEntity
        {
            AuthorId = new UniqueEntityId(authorId),
            QuestionId = new UniqueEntityId(questionId),
            Content = content,
            IsClosed = isClosed,
            Attachments = attachments?.Count > 0
                ? attachments.Select((att) => AttachmentEntity.Create(new UniqueEntityId(questionId), AttachmentOwnerTypeEnum.Answer, att.Title, att.Link))
                : []
        };
    }
}