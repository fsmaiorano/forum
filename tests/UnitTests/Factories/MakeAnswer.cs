using Forum.Application.UseCases.Answer.CreateAnswer;
using Forum.Domain.Entities.ValuesObjects;

namespace UnitTests.Factories;

public static class MakeAnswer
{
    public static AnswerEntity Create(
        UniqueEntityId? authorId = null!,
        UniqueEntityId? questionId = null!,
        string? content = null!,
        bool? isClosed = false)
    {
        var faker = new Bogus.Faker();
        return AnswerEntity.Create(
            authorId: authorId ?? new UniqueEntityId(),
            questionId: questionId ?? new UniqueEntityId(),
            content: content ?? faker.Lorem.Paragraph(),
            isClosed: isClosed ?? false);
    }

    public static CreateAnswerCommand CreateAnswerCommand(
        UniqueEntityId? questionId,
        UniqueEntityId? authorId,
        string? content,
        bool isClosed = false,
        List<AttachmentEntity>? attachments = null)
    {
        var faker = new Bogus.Faker();
        return new CreateAnswerCommand(
            AuthorId: authorId ?? new UniqueEntityId(),
            QuestionId: questionId ?? new UniqueEntityId(),
            Content: content ?? faker.Lorem.Paragraph(),
            Attachments: attachments ?? []
        );
    }
}