using UnitTests.Factories;

namespace UnitTests.Repositories;

public class QuestionRepositoryUnitTest(DatabaseFixture fixture) : BaseTest(fixture)
{
    [Fact]
    public async Task Create_ShouldAddQuestionToContext()
    {
        var repository = new QuestionRepository(Context);

        var question = MakeQuestion.Create();
        await repository.Create(question);
        await Context.SaveChangesAsync();

        var storedQuestion = await Context.Question
            .FirstOrDefaultAsync(q => q.Id == question.Id);

        Assert.NotNull(storedQuestion);
        Assert.Equal(question.Title, storedQuestion.Title);
        Assert.Equal(question.Content, storedQuestion.Content);
    }
}