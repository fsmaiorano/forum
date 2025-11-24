using Forum.Application.UseCases.Question.UpdateQuestion;
using Forum.Domain.Entities.ValuesObjects;

namespace UnitTests.Factories;

public static class MakeQuestion
{
    public static QuestionEntity Create(
        string? authorId = null,
        string? title = null,
        string? content = null,
        string? slug = null)
    {
        var faker = new Bogus.Faker();
        return QuestionEntity.Create(
            authorId ?? faker.Random.Uuid().ToString(),
            title ?? faker.Lorem.Sentence(3),
            content ?? faker.Lorem.Paragraph(),
            slug ?? faker.Lorem.Slug());
    }

    public static CreateQuestionCommand CreateQuestionCommand(
        string? authorId = null,
        string? title = null,
        string? content = null,
        string? slug = null,
        List<AttachmentEntity>? attachments = null)
    {
        var faker = new Bogus.Faker();
        return new CreateQuestionCommand(
            Title: title ?? faker.Lorem.Sentence(3),
            Content: content ?? faker.Lorem.Paragraph(),
            AuthorId: authorId ?? faker.Random.Uuid().ToString(),
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
            QuestionId: questionId,
            AuthorId: authorId,
            Title: title ?? faker.Lorem.Sentence(3),
            Content: content ?? faker.Lorem.Paragraph(),
            Slug: slug ?? faker.Lorem.Slug(),
            Attachments: attachments ?? []
        );
    }
}