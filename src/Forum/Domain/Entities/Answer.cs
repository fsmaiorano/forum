using Forum.BuildingBlocks.Base;
using Forum.Domain.Enums;

namespace Forum.Domain.Entities;

public sealed record Answer : Entity
{
    public UniqueEntityId AuthorId { get; private set; } = null!;
    public UniqueEntityId QuestionId { get; private set; } = null!;
    public string Content { get; private set; } = null!;
    public bool IsClosed { get; private set; }
    public IEnumerable<Attachment> Attachments { get; private set; } = [];


    public static Answer Create(
        string authorId,
        string questionId,
        string content,
        bool isClosed = false,
        List<Attachment>? attachments = null
    )
    {
        var id = new UniqueEntityId();
        return new Answer
        {
            AuthorId = new UniqueEntityId(authorId),
            QuestionId = new UniqueEntityId(questionId),
            Content = content,
            IsClosed = isClosed,
            Attachments = attachments?.Count > 0
                ? attachments.Select((att) => Attachment.Create(id.ToString(), AttachmentOwnerType.Answer, att.Title, att.Link))
                : []
        };
    }
}