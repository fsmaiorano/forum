using BuildingBlocks.Base;
using Forum.Application.UseCases.Question.GetQuestionById;
using Forum.Application.UseCases.Question.GetQuestionsByAuthorId;
using Forum.Domain.Enums;
using Forum.UnitTest.Factories;
using Forum.UnitTest.Fixtures;

namespace Forum.UnitTest.Application.UseCases.Question;

public class GetQuestionsByAuthorIdUnitTest(TestFixture fixture) : BaseTest(fixture)
{
    [Fact]
    public async Task GeQuestionByAuthorIdUseCaseHandler_ShouldReturnQuestions()
    {
        var question = MakeQuestion.Create();
        for (var i = 1; i <= 6; i++)
        {
            var questionWithSameAuthor = MakeQuestion.Create(question.AuthorId);
            await Context.Question.AddAsync(questionWithSameAuthor);
        }

        await Context.SaveChangesAsync();

        var questionRepository = new QuestionRepository(Context, DomainEventDispatcher);
        var attachmentRepository = new AttachmentRepository(Context);
        var loggerMock = CreateLoggerMock<GetQuestionsByAuthorIdUseCase>();
        var useCase = new GetQuestionsByAuthorIdUseCase(loggerMock.Object, questionRepository, attachmentRepository);

        var query = new GetQuestionsByAuthorIdQuery(question.AuthorId);
        var result = await useCase.GetQuestionsByAuthorIdUseCaseHandler(query);

        Assert.NotNull(result);
        Assert.Equal(6, result.Value?.Questions.Count());
        Assert.Equal(question.AuthorId, result.Value?.Questions.First().AuthorId);
    }
}