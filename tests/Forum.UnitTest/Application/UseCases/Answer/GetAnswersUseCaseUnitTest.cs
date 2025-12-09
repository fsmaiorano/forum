using BuildingBlocks.Base;
using Forum.Application.UseCases.Answer.GetAnswers;
using Forum.Domain.Enums;
using Forum.UnitTest.Factories;
using Forum.UnitTest.Fixtures;

namespace Forum.UnitTest.Application.UseCases.Answer;

public class GetAnswersUseCaseUnitTest(TestFixture fixture) : BaseTest(fixture)
{
    [Fact]
    public async Task GetAnswersUseCaseHandler_ShouldReturnAnswers()
    {
        var attachments = new List<AttachmentEntity>();
        for (var i = 1; i <= 2; i++)
            attachments.Add(MakeAttachment.Create(new UniqueEntityId(), AttachmentOwnerTypeEnum.Question));

        var question = MakeQuestion.Create();
        question.Attachments.AddRange(attachments);

        await Context.Question.AddAsync(question);
        await Context.SaveChangesAsync();
        
        var answer = MakeAnswer.Create(questionId: question.Id);
        await Context.Answer.AddAsync(answer);
        await Context.SaveChangesAsync();

        var repository = new AnswerRepository(Context, DomainEventDispatcher);
        var loggerMock = CreateLoggerMock<GetAnswersUseCase>();
        var useCase = new GetAnswersUseCase(loggerMock.Object, repository);

        var query = new GetAnswersQuery(question.Id.ToString());
        var result = await useCase.GetAnswersUseCaseHandler(query);

        Assert.NotNull(result);
        Assert.Equivalent(1, result.Value.Answers.Count());
    }
}