using BuildingBlocks.Base;
using Forum.Application.UseCases.Question.PatchQuestionSetBestAnswer;
using Forum.Application.UseCases.Question.UpdateQuestion;
using Forum.UnitTests.Factories;
using Forum.UnitTests.Fixtures;

namespace Forum.UnitTests.Application.UseCases.Question;

public class PatchQuestionSetBestAnswerUnitTest(TestFixture fixture) : BaseTest(fixture)
{
    [Fact]
    public async Task PatchQuestionSetBestAnswer_ShouldPatchBestAnswerId()
    {
        var repository = new QuestionRepository(Context, DomainEventDispatcher);
        var loggerMock = CreateLoggerMock<PatchQuestionSetBestAnswerUseCase>();
        var useCase = new PatchQuestionSetBestAnswerUseCase(loggerMock.Object, repository);

        var question = MakeQuestion.Create();
        await repository.Create(question);

        var command = new PatchQuestionSetBestAnswerCommand(question.Id, question.AuthorId, new UniqueEntityId());

        await useCase.PatchQuestionSetBestAnswerUseCaseHandler(command);
        var updatedQuestion = await repository.FindById(question.Id);

        Assert.NotNull(updatedQuestion);
        Assert.NotNull(updatedQuestion.BestAnswerId);
    }
}