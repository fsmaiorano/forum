namespace UnitTests.Application.UseCases.Question;

public class CreateQuestionUnitTest(DatabaseFixture fixture) : IClassFixture<DatabaseFixture>
{
    private readonly ForumDbContext _context = fixture.Context;

    [Fact]
    public async Task CreateQuestionUseCaseHandler_ShouldCreateQuestion()
    {
        var repo = new QuestionRepository(_context);
        var loggerMock = new Mock<ILogger<CreateQuestionUseCase>>();
        var useCase = new CreateQuestionUseCase(loggerMock.Object, repo);

        var command = new CreateQuestionCommand(
            Title: "in-memory title",
            Content: "in-memory content",
            AuthorId: new UniqueEntityId().ToString()
        );

        var result = await useCase.CreateQuestionUseCaseHandler(command);

        var storedQuestion = await _context.Question
            .FirstOrDefaultAsync(q => q.Id == UniqueEntityId.Of(result.QuestionId));

        Assert.NotNull(storedQuestion);
        Assert.Equal(command.Title, storedQuestion!.Title);
        Assert.Equal(command.Content, storedQuestion.Content);
    }
}