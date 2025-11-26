using Forum.Application.UseCases.Answer.UpdateAnswer;
using Forum.Application.UseCases.Question.UpdateQuestion;
using UnitTests.Factories;

namespace UnitTests.Application.UseCases.Answer;

public class UpdateAnswerUnitTest(TestFixture fixture) : BaseTest(fixture)
{
    [Fact]
    public async Task UpdateAnswerUseCaseHandler_ShouldUpdateAnswer()
    {
        var answerRepository = new AnswerRepository(Context);
        var questionRepository = new QuestionRepository(Context);
        var attachmentRepository = new AttachmentRepository(Context);
        var loggerMock = CreateLoggerMock<UpdateAnswerUseCase>();
        var useCase = new UpdateAnswerUseCase(loggerMock.Object, answerRepository, attachmentRepository);

        var question = MakeQuestion.Create();
        await questionRepository.Create(question);
        
        var answer = MakeAnswer.Create(questionId: question.Id);
        await answerRepository.Create(answer);

        var command = MakeAnswer.UpdateAnswerCommand(
            answerId: answer.Id.ToString(),
            authorId: answer.AuthorId.ToString(),
            content: question.Content + "_Updated"
        );

        await useCase.UpdateAnswerUseCaseHandler(command);
        var updatedQuestion = await answerRepository.FindById(answer.Id);

        Assert.NotNull(updatedQuestion);
        Assert.Equal(command.Content, updatedQuestion.Content);
    }
}