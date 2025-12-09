using BuildingBlocks.Base;
using Forum.Application.UseCases.Question.GetQuestionById;
using Forum.Domain.Enums;
using Forum.UnitTest.Factories;
using Forum.UnitTest.Fixtures;

namespace Forum.UnitTest.Application.UseCases.Question;

public class GetQuestionByIdUseCaseUnitTest(TestFixture fixture) : BaseTest(fixture)
{
    [Fact]
    public async Task GeQuestionByIdUseCaseHandler_ShouldReturnQuestion()
    {
        var attachments = new List<AttachmentEntity>();
        for (var i = 1; i <= 2; i++)
            attachments.Add(MakeAttachment.Create(new UniqueEntityId(), AttachmentOwnerTypeEnum.Question));

        var question = MakeQuestion.Create();
        question.Attachments.AddRange(attachments);

        await Context.Question.AddAsync(question);
        await Context.SaveChangesAsync();

        var questionRepository = new QuestionRepository(Context, DomainEventDispatcher);
        var attachmentRepository = new AttachmentRepository(Context);
        var loggerMock = CreateLoggerMock<GetQuestionByIdUseCase>();
        var useCase = new GetQuestionByIdUseCase(loggerMock.Object, questionRepository, attachmentRepository);

        var command = new GetQuestionByIdQuery(question.Id);
        var result = await useCase.GetQuestionByIdUseCaseHandler(command);

        Assert.NotNull(result);
        Assert.Equal(question.Id.ToString(), result.Value?.Question?.Id.ToString());
    }
}