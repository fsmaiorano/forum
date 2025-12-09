using Forum.Application.UseCases.Answer.DeleteAnswer;
using Forum.UnitTest.Base;
using Forum.UnitTest.Factories;
using Forum.UnitTest.Fixtures;

namespace Forum.UnitTest.Application.UseCases.Answer;

public class DeleteAnswerUseCaseUnitTest(TestFixture fixture) : BaseTest(fixture)
{
    [Fact]
    public async Task DeleteAnswerUseCaseHandler_ShouldDeleteAnswer()
    {
        var answerRepository = new AnswerRepository(Context, DomainEventDispatcher);
        var questionRepository = new QuestionRepository(Context,DomainEventDispatcher);
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