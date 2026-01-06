using BuildingBlocks.Base;
using Forum.Application.UseCases.Question.UpdateQuestion;
using Forum.Endpoints.Dtos;
using Forum.Endpoints.Question;

namespace Forum.UnitTest.Factories;

public static class MakeQuestion
{
    public static QuestionEntity Create(
        UniqueEntityId? authorId = null,
        string? title = null,
        string? content = null,
        string? slug = null)
    {
        var faker = new Bogus.Faker();
        return QuestionEntity.Create(
            authorId ?? new UniqueEntityId(),
            title ?? faker.Lorem.Sentence(3),
            content ?? faker.Lorem.Paragraph(),
            slug ?? faker.Lorem.Slug());
    }

    public static CreateQuestionRequest CreateQuestionRequest(
        string? authorId = null,
        string? title = null,
        string? content = null,
        List<AttachmentRequest>? attachments = null)
    {
        var faker = new Bogus.Faker();
        return new CreateQuestionRequest(
            title ?? faker.Lorem.Sentence(3),
            content ?? faker.Lorem.Paragraph(),
            authorId ?? faker.Random.Uuid().ToString(),
            Attachments: attachments ?? []
        );
    }

    public static CreateQuestionCommand CreateQuestionCommand(
        UniqueEntityId? authorId = null,
        string? title = null,
        string? content = null,
        string? slug = null,
        List<AttachmentEntity>? attachments = null)
    {
        var faker = new Bogus.Faker();
        return new CreateQuestionCommand(
            Title: title ?? faker.Lorem.Sentence(3),
            Content: content ?? faker.Lorem.Paragraph(),
            AuthorId: authorId ?? new UniqueEntityId(),
            Slug: slug ?? faker.Lorem.Slug(),
            Attachments: attachments ?? []
        );
    }

    public static UpdateQuestionRequest UpdateQuestionRequest(
        string questionId,
        string authorId,
        string? title = null,
        string? content = null,
        string? slug = null,
        List<AttachmentRequest>? attachments = null)
    {
        var faker = new Bogus.Faker();
        return new UpdateQuestionRequest(
            QuestionId: questionId,
            AuthorId: authorId,
            Title: title ?? faker.Lorem.Sentence(3),
            Content: content ?? faker.Lorem.Paragraph(),
            Slug: slug ?? faker.Lorem.Slug(),
            Attachments: attachments ?? []
        );
    }

    public static UpdateQuestionCommand UpdateQuestionCommand(
        string questionId,
        string authorId,
        string? title = null,
        string? content = null,
        string? slug = null,
        List<AttachmentEntity>? attachments = null)
    {
        var faker = new Bogus.Faker();
        return new UpdateQuestionCommand(
            QuestionId: new UniqueEntityId(questionId),
            AuthorId: new UniqueEntityId(authorId),
            Title: title ?? faker.Lorem.Sentence(3),
            Content: content ?? faker.Lorem.Paragraph(),
            Slug: slug ?? faker.Lorem.Slug(),
            Attachments: attachments ?? []
        );
    }

    public static PatchQuestionSetBestAnswerRequest PatchQuestionSetBestAnswerRequest(string questionId,
        string bestAnswerId)
    {
        return new PatchQuestionSetBestAnswerRequest(questionId, bestAnswerId);
    }
}