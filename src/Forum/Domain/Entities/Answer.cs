namespace Forum.Domain.Entities;

public record Answer : Entity
{
    public UniqueEntityId AuthorId { get; private set; } = null!;
    public UniqueEntityId QuestionId { get; private set; } = null!;
    public string Content { get; private set; } = null!;
    public bool IsClosed { get; private set; }

    public static Answer Create(
        string authorId,
        string questionId,
        string content,
        bool isClosed = false
    )
    {
        return new Answer
        {
            AuthorId = new UniqueEntityId(authorId),
            QuestionId = new UniqueEntityId(questionId),
            Content = content,
            IsClosed = isClosed
        };
    }
}