using Forum.Application.UseCases.Question.UpdateQuestion;
using Forum.BuildingBlocks.Exceptions;
using UnitTests.Factories;

namespace UnitTests.Application.UseCases.Question;

public class UpdateQuestionUnitTest(DatabaseFixture fixture) : BaseTest(fixture)
{
    [Fact]
    public async Task UpdateQuestionUseCaseHandler_ShouldUpdateQuestion()
    {
        var repository = new QuestionRepository(Context);
        var attachmentRepository = new AttachmentRepository(Context);
        var loggerMock = CreateLoggerMock<UpdateQuestionUseCase>();
        var useCase = new UpdateQuestionUseCase(loggerMock.Object, repository, attachmentRepository);

        var question = MakeQuestion.Create();
        await repository.Create(question);

        var command = MakeQuestion.UpdateQuestionCommand(
            questionId: question.Id.ToString(),
            authorId: question.AuthorId.ToString(),
            title: question.Title + "_Updated",
            content: question.Content + "_Updated",
            slug: question.Slug?.Value
        );

        await useCase.UpdateQuestionUseCaseHandler(command);
        var updatedQuestion = await repository.FindById(question.Id.ToString());

        Assert.NotNull(updatedQuestion);
        Assert.Equal(command.Title, updatedQuestion.Title);
        Assert.Equal(command.Content, updatedQuestion.Content);
    }

    [Fact]
    public async Task UpdateQuestionUseCaseHandler_ShouldThrowNotFoundException_WhenQuestionDoesNotExist()
    {
        var repository = new QuestionRepository(Context);
        var attachmentRepository = new AttachmentRepository(Context);
        var loggerMock = CreateLoggerMock<UpdateQuestionUseCase>();
        var useCase = new UpdateQuestionUseCase(loggerMock.Object, repository, attachmentRepository);

        var command = MakeQuestion.UpdateQuestionCommand(
            questionId: Guid.NewGuid().ToString(),
            authorId: Guid.NewGuid().ToString(),
            title: "Sample Title",
            content: "Sample Content"
        );

        await Assert.ThrowsAsync<NotFoundException>(async () =>
        {
            await useCase.UpdateQuestionUseCaseHandler(command);
        });
    }
}