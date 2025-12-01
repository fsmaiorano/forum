using BuildingBlocks.Base;
using Forum.Application.UseCases.Answer.CreateAnswer;
using Forum.Application.UseCases.Answer.UpdateAnswer;
using Forum.Endpoints.Answer;
using Forum.Endpoints.Dtos;

namespace Forum.UnitTests.Factories;

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
        UniqueEntityId? questionId = null!,
        UniqueEntityId? authorId = null!,
        string? content = null!,
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

    public static CreateAnswerRequest CreateAnswerRequest(string? questionId, string? authorId = null,
        string? content = null,
        List<AttachmentRequest>? attachments = null)
    {
        var faker = new Bogus.Faker();
        return new CreateAnswerRequest(
            QuestionId: questionId ?? new UniqueEntityId().ToString(),
            AuthorId: authorId ?? new UniqueEntityId().ToString(),
            Content: content ?? faker.Lorem.Paragraph(),
            Attachments: attachments ?? []
        );
    }

    public static UpdateAnswerCommand UpdateAnswerCommand(
        string answerId,
        string authorId,
        string? content = null!,
        bool isClosed = false,
        List<AttachmentEntity>? attachments = null)
    {
        var faker = new Bogus.Faker();
        return new UpdateAnswerCommand(
            AnswerId: new UniqueEntityId(answerId),
            AuthorId: new UniqueEntityId(authorId),
            Content: content ?? faker.Lorem.Paragraph(),
            Attachments: attachments ?? []
        );
    }

    public static UpdateAnswerRequest UpdateAnswerRequest(string answerId,
        string authorId,
        string? content = null,
        bool? isClosed = false,
        List<AttachmentRequest>? attachments = null)
    {
        var faker = new Bogus.Faker();
        return new UpdateAnswerRequest(
            AnswerId: answerId,
            AuthorId: authorId,
            Content: content ?? faker.Lorem.Paragraph(),
            Attachments: attachments ?? []
        );
    }
}