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
}