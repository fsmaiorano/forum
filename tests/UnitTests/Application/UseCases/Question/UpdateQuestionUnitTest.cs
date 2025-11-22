using Forum.Application.UseCases.Question.UpdateQuestion;
using Forum.Domain.Entities.ValuesObjects;
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
            content: question.Content,
            slug: question.Slug?.Value
        );
        
        var result = await useCase.UpdateQuestionUseCaseHandler(command);
        var updatedQuestion = await repository.FindById(question.Id.ToString());
        
        Assert.NotNull(updatedQuestion);
        Assert.Equal(command.Title, updatedQuestion.Title);
        Assert.Equal(command.Content, updatedQuestion.Content);
    }
}