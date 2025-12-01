using BuildingBlocks.Base;
using BuildingBlocks.Events;
using BuildingBlocks.Extensions;
using Forum.Domain.Enums;
using Forum.Domain.Events;

namespace Forum.Domain.Entities;

public sealed record AnswerEntity : Aggregate
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

        var answer =  new AnswerEntity
        {
            Id = new UniqueEntityId(),
            AuthorId = authorId,
            QuestionId = questionId,
            Content = content,
            IsClosed = isClosed,
            Attachments = attachments ?? AttachmentList.Create(),
        };
        
        answer.AddDomainEvent(new AnswerCreatedEvent(answer));
        
        return answer;
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
        return string.IsNullOrEmpty(content) ? string.Empty : content.Excerpt(maxLength);
    }
}