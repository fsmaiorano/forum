using Forum.Application.UseCases.Answer.UpdateAnswer;
using Forum.Domain.Enums;
using Forum.UnitTests.Factories;
using Forum.UnitTests.Fixtures;

namespace Forum.UnitTests.Application.UseCases.Answer;

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

    [Fact]
    public async Task UpdateAnswerUseCaseHandler_ShouldUpdateAnswerWithAttachments()
    {
        var answerRepository = new AnswerRepository(Context);
        var questionRepository = new QuestionRepository(Context);
        var attachmentRepository = new AttachmentRepository(Context);
        var loggerMock = CreateLoggerMock<UpdateAnswerUseCase>();
        var useCase = new UpdateAnswerUseCase(loggerMock.Object, answerRepository, attachmentRepository);

        var question = MakeQuestion.Create();
        await questionRepository.Create(question);

        var answer = MakeAnswer.Create(questionId: question.Id);
        answer.Attachments.Add(
            MakeAttachment.Create(answer.Id, title: "1", ownerTypeEnum: AttachmentOwnerTypeEnum.Answer)
        );
        await answerRepository.Create(answer);

        var command = MakeAnswer.UpdateAnswerCommand(
            answerId: answer.Id.ToString(),
            authorId: answer.AuthorId.ToString(),
            content: question.Content + "_Updated",
            attachments:
            [
                MakeAttachment.Create(answer.Id, title: "2", ownerTypeEnum: AttachmentOwnerTypeEnum.Answer),
                MakeAttachment.Create(answer.Id, title: "3", ownerTypeEnum: AttachmentOwnerTypeEnum.Answer),
                MakeAttachment.Create(answer.Id, title: "4", ownerTypeEnum: AttachmentOwnerTypeEnum.Answer)
            ]
        );

        await useCase.UpdateAnswerUseCaseHandler(command);
        var updatedQuestion = await answerRepository.FindById(answer.Id);

        Assert.NotNull(updatedQuestion);
        Assert.Equal(command.Content, updatedQuestion.Content);
    }
}