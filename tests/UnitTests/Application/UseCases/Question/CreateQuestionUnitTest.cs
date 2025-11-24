using Forum.Domain;
using Forum.Domain.Entities.ValuesObjects;
using Forum.Domain.Enums;
using UnitTests.Factories;

namespace UnitTests.Application.UseCases.Question;

public class CreateQuestionUnitTest(DatabaseFixture fixture) : BaseTest(fixture)
{
    [Fact]
    public async Task CreateQuestionUseCaseHandler_ShouldCreateQuestion()
    {
        var repository = new QuestionRepository(Context);
        var attachmentRepository = new AttachmentRepository(Context);
        var loggerMock = CreateLoggerMock<CreateQuestionUseCase>();
        var useCase = new CreateQuestionUseCase(loggerMock.Object, repository, attachmentRepository);

        var command = MakeQuestion.CreateQuestionCommand();
        var result = await useCase.CreateQuestionUseCaseHandler(command);

        var storedQuestion = await Context.Question
            .FirstOrDefaultAsync(q => q.Id == UniqueEntityId.Of(result.Value.QuestionId));

        Assert.NotNull(storedQuestion);
        Assert.Equal(command.Title, storedQuestion.Title);
        Assert.Equal(command.Content, storedQuestion.Content);
    }

    [Fact]
    public async Task CreateQuestionUseCaseHandler_ShouldCreateQuestionWithAttachments()
    {
        var questionRepository = new QuestionRepository(Context);
        var attachmentRepository = new AttachmentRepository(Context);
        var loggerMock = CreateLoggerMock<CreateQuestionUseCase>();
        var useCase = new CreateQuestionUseCase(loggerMock.Object, questionRepository, attachmentRepository);

        var attachments = new List<AttachmentEntity>();
        for (var i = 1; i <= 2; i++)
            attachments.Add(MakeAttachment.Create(new UniqueEntityId(), AttachmentOwnerTypeEnum.Question));

        var command = MakeQuestion.CreateQuestionCommand(attachments: attachments);
        var result = await useCase.CreateQuestionUseCaseHandler(command);

        var storedQuestion = await Context.Question
            .FirstOrDefaultAsync(q => q.Id == UniqueEntityId.Of(result.Value.QuestionId));

        var storedAttachments = Context.Attachment.Where(a => a.OwnerId.Equals(storedQuestion!.Id)).ToList();

        Assert.NotNull(storedQuestion);
        Assert.Equal(command.Title, storedQuestion.Title);
        Assert.Equal(command.Content, storedQuestion.Content);
        Assert.Equal(2, storedAttachments.Count);
    }
}