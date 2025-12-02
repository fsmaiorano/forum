using Forum.Application.UseCases.Answer.DeleteAnswer;
using Forum.UnitTests.Factories;
using Forum.UnitTests.Fixtures;

namespace Forum.UnitTests.Application.UseCases.Answer;

public class DeleteAnswerUnitTest(TestFixture fixture) : BaseTest(fixture)
{
    [Fact]
    public async Task DeleteAnswerUseCaseHandler_ShouldDeleteAnswer()
    {
        var answerRepository = new AnswerRepository(Context);
        var questionRepository = new QuestionRepository(Context);
        var attachmentRepository = new AttachmentRepository(Context);
        var loggerMock = CreateLoggerMock<DeleteAnswerUseCase>();
        var useCase = new DeleteAnswerUseCase(loggerMock.Object, answerRepository, attachmentRepository);
        var question = MakeQuestion.Create();
        await questionRepository.Create(question);

        var answer = MakeAnswer.Create(questionId: question.Id);
        await answerRepository.Create(answer);
        
        Thread.Sleep(1000);

        var command = new DeleteAnswerCommand(answer.Id);
        await useCase.DeleteAnswerUseCaseHandler(command);

        var storedAnswer = await answerRepository.FindById(answer.Id);
        var storedAttachments = await attachmentRepository.FindByOwnerId(answer.Id);

        Assert.Null(storedAnswer);
        Assert.Empty(storedAttachments);
    }
}