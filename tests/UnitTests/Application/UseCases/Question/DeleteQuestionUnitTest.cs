using Forum.Domain.Entities.ValuesObjects;
using Forum.Domain.Enums;
using UnitTests.Factories;

namespace UnitTests.Application.UseCases.Question;

public class DeleteQuestionUnitTest(DatabaseFixture fixture) : BaseTest(fixture)
{
    [Fact]
    public async Task DeleteQuestionUseCaseHandler_ShouldDeleteQuestion()
    {
        var repository = new QuestionRepository(Context);
        var attachmentRepository = new AttachmentRepository(Context);
        var loggerMock = CreateLoggerMock<CreateQuestionUseCase>();
        var useCase = new CreateQuestionUseCase(loggerMock.Object, repository, attachmentRepository);

        var command = MakeQuestion.CreateQuestionCommand();
        var result = await useCase.CreateQuestionUseCaseHandler(command);

        var storedQuestion = await Context.Question
            .FirstOrDefaultAsync(q => q.Id == UniqueEntityId.Of(result.Value.QuestionId));

        await repository.Delete(storedQuestion!);

        storedQuestion = await Context.Question
            .FirstOrDefaultAsync(q => q.Id == UniqueEntityId.Of(result.Value.QuestionId));

        Assert.Null(storedQuestion);
    }
    
    [Fact]
    public async Task DeleteQuestionUseCasehandler_ShouldDeleteQuestionWithAttachments()
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

        var storedQuestionId = UniqueEntityId.Of(result.Value.QuestionId);
        var storedQuestion = await Context.Question
            .FirstOrDefaultAsync(q => q.Id == storedQuestionId);

        await attachmentRepository.DeleteByQuestionId(storedQuestion!.Id);
        await questionRepository.Delete(storedQuestion!);

        storedQuestion = await Context.Question
            .FirstOrDefaultAsync(q => q.Id == storedQuestionId);

        var storedAttachments = Context.Attachment.Where(a => a.OwnerId.Equals(storedQuestionId)).ToList();

        Assert.Null(storedQuestion);
        Assert.Empty(storedAttachments);
    }
}