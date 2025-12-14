using BuildingBlocks.Base;
using Forum.Application.UseCases.Question.GetQuestions;
using Forum.Domain.Enums;
using Forum.UnitTest.Factories;
using Forum.UnitTest.Fixtures;

namespace Forum.UnitTest.Application.UseCases.Question;

public class GetQuestionsUseCaseUnitTest(TestFixture fixture) : BaseTest(fixture)
{
    [Fact]
    public async Task GetQuestionsUseCaseHandler_ShouldReturnQuestions()
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
        var loggerMock = CreateLoggerMock<GetQuestionsUseCase>();
        var useCase = new GetQuestionsUseCase(loggerMock.Object, questionRepository, attachmentRepository);

        var result = await useCase.GetQuestionsUseCaseHandler();

        Assert.NotNull(result);
        Assert.Equivalent(1, result.Value.Questions.Count());
    }
}