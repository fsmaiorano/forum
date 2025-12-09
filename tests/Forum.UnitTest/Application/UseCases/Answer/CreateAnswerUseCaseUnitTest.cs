using BuildingBlocks.Base;
using BuildingBlocks.Messaging.DomainEvents;
using Forum.Application.UseCases.Answer.CreateAnswer;
using Forum.Domain.Enums;
using Forum.Domain.Events;
using Forum.UnitTest.Base;
using Forum.UnitTest.Factories;
using Forum.UnitTest.Fixtures;

namespace Forum.UnitTest.Application.UseCases.Answer;

public class CreateAnswerUseCaseUnitTest(TestFixture fixture) : BaseTest(fixture)
{
    [Fact]
    public async Task CreateAnswerUseCaseHandler_ShouldCreateAnswer()
    {
        var answerRepository = new AnswerRepository(Context, DomainEventDispatcher);
        var attachmentRepository = new AttachmentRepository(Context);
        var loggerMock = CreateLoggerMock<CreateAnswerUseCase>();
        var useCase = new CreateAnswerUseCase(loggerMock.Object, answerRepository, attachmentRepository);

        var question = MakeQuestion.Create();
        await Context.Question.AddAsync(question);
        await Context.SaveChangesAsync();

        Thread.Sleep(100);

        var command = MakeAnswer.CreateAnswerCommand(question.Id);
        var result = await useCase.CreateAnswerUseCaseHandler(command);

        var storedAnswer =
            await Context.Answer.FirstOrDefaultAsync(a => a.Id == UniqueEntityId.Of(result.Value.AnswerId));

        Assert.NotNull(storedAnswer);
        Assert.Equal(command.Content, storedAnswer.Content);
        Assert.Equal(command.QuestionId, storedAnswer.QuestionId);
    }
    
    [Fact]
    public async Task CreateAnswerUseCaseHandler_ShouldRaiseDomainEvent()
    {
        ClearDomainEvents();
        
        var answerRepository = new AnswerRepository(Context, DomainEventDispatcher);
        var attachmentRepository = new AttachmentRepository(Context);
        var loggerMock = CreateLoggerMock<CreateAnswerUseCase>();
        var useCase = new CreateAnswerUseCase(loggerMock.Object, answerRepository, attachmentRepository);

        var question = MakeQuestion.Create();
        await Context.Question.AddAsync(question);
        await Context.SaveChangesAsync();

        var command = MakeAnswer.CreateAnswerCommand(question.Id);

        var result = await useCase.CreateAnswerUseCaseHandler(command);

        var dispatcher = GetTestDispatcher();
        Assert.True(dispatcher.HasEvent<AnswerCreatedEvent>(), "AnswerCreatedEvent should have been dispatched");
        
        var domainEvent = dispatcher.GetEvent<AnswerCreatedEvent>();
        Assert.NotNull(domainEvent);
        Assert.Equal(UniqueEntityId.Of(result.Value.AnswerId), domainEvent.GetAggregateId());
        Assert.True(domainEvent.OccurredAt <= DateTime.UtcNow);
    }

    [Fact]
    public async Task CreateAnswerUseCaHandler_ShouldCreateAnswerWithAttachment()
    {
        var answerRepository = new AnswerRepository(Context,DomainEventDispatcher);
        var attachmentRepository = new AttachmentRepository(Context);
        var loggerMock = CreateLoggerMock<CreateAnswerUseCase>();
        var useCase = new CreateAnswerUseCase(loggerMock.Object, answerRepository, attachmentRepository);

        var attachments = new List<AttachmentEntity>();
        for (var i = 1; i <= 2; i++)
            attachments.Add(MakeAttachment.Create(new UniqueEntityId(), AttachmentOwnerTypeEnum.Answer));

        var question = MakeQuestion.Create();
        await Context.Question.AddAsync(question);
        await Context.SaveChangesAsync();

        var command = MakeAnswer.CreateAnswerCommand(question.Id, attachments: attachments);
        var result = await useCase.CreateAnswerUseCaseHandler(command);

        var storedAnswer =
            await Context.Answer.FirstOrDefaultAsync(a => a.Id == UniqueEntityId.Of(result.Value.AnswerId));

        var storedAttachments = Context.Attachment.Where(a => a.OwnerId.Equals(storedAnswer!.Id)).ToList();

        Assert.NotNull(storedAnswer);
        Assert.Equal(command.Content, storedAnswer.Content);
        Assert.Equal(command.QuestionId, storedAnswer.QuestionId);
        Assert.Equal(2, storedAttachments.Count);
    }
}