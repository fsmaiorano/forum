using UnitTests.Factories;

namespace UnitTests.Application.UseCases.Question;

public class CreateQuestionUnitTest(DatabaseFixture fixture) : BaseTest(fixture)
{
    [Fact]
    public async Task CreateQuestionUseCaseHandler_ShouldCreateQuestionWithSuccess()
    {
        var repo = new QuestionRepository(Context);
        var loggerMock = CreateLoggerMock<CreateQuestionUseCase>();
        var useCase = new CreateQuestionUseCase(loggerMock.Object, repo);

        var command = MakeQuestion.CreateQuestionCommand();
        var result = await useCase.CreateQuestionUseCaseHandler(command);

        var storedQuestion = await Context.Question
            .FirstOrDefaultAsync(q => q.Id == UniqueEntityId.Of(result.Value.QuestionId));

        Assert.NotNull(storedQuestion);
        Assert.Equal(command.Title, storedQuestion.Title);
        Assert.Equal(command.Content, storedQuestion.Content);
    }
}