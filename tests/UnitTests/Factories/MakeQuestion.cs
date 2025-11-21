namespace UnitTests.Factories;

public static class MakeQuestion
{
    public static Question Create(
        string? authorId = null,
        string? title = null,
        string? content = null,
        string? slug = null)
    {
        var faker = new Bogus.Faker();
        return Question.Create(
            authorId ?? faker.Random.Uuid().ToString(),
            title ?? faker.Lorem.Sentence(3),
            content ?? faker.Lorem.Paragraph(),
            slug ?? faker.Lorem.Slug());
    }

    public static CreateQuestionCommand CreateQuestionCommand(
        string? authorId = null,
        string? title = null,
        string? content = null)
    {
        var faker = new Bogus.Faker();
        return new CreateQuestionCommand(
            Title: title ?? faker.Lorem.Sentence(3),
            Content: content ?? faker.Lorem.Paragraph(),
            AuthorId: authorId ?? faker.Random.Uuid().ToString()
        );
    }
}