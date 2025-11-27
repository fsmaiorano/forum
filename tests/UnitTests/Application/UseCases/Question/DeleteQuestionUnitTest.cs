using Forum.Domain.Entities.ValuesObjects;
using Forum.Domain.Enums;
using UnitTests.Factories;

namespace UnitTests.Application.UseCases.Question;

public class DeleteQuestionUnitTest(TestFixture fixture) : BaseTest(fixture)
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

        var questionId = UniqueEntityId.Of(result.Value.QuestionId);
        var storedQuestion = await repository.FindById(questionId);

        await repository.Delete(storedQuestion!);

        var deletedQuestion = await Context.Question
            .AsNoTracking()
            .FirstOrDefaultAsync(q => q.Id == questionId);

        Assert.Null(deletedQuestion);
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
        
        var storedQuestion = await questionRepository.FindById(storedQuestionId);

        await attachmentRepository.DeleteByOwnerId(storedQuestion!.Id);
        await questionRepository.Delete(storedQuestion!);

        var deletedQuestion = await Context.Question
            .AsNoTracking()
            .FirstOrDefaultAsync(q => q.Id == storedQuestionId);

        var storedAttachments = await Context.Attachment
            .AsNoTracking()
            .Where(a => a.OwnerId.Equals(storedQuestionId))
            .ToListAsync();

        Assert.Null(deletedQuestion);
        Assert.Empty(storedAttachments);
    }
}