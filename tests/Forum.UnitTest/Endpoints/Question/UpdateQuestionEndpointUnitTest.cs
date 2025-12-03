using System.Net;
using Forum.UnitTest.Base;
using Forum.UnitTest.Factories;
using Forum.UnitTest.Fixtures;

namespace Forum.UnitTest.Endpoints.Question;

public class UpdateQuestionEndpointUnitTest(TestFixture fixture)
    : BaseTest(fixture)
{
    [Fact]
    public async Task UpdateQuestionEndpoint_ShouldReturn204()
    {
        var repository = new QuestionRepository(Context, DomainEventDispatcher);

        var question = MakeQuestion.Create();
        await repository.Create(question);

        var request = MakeQuestion.UpdateQuestionRequest(
            questionId: question.Id.ToString(),
            authorId: question.AuthorId.ToString(),
            title: question.Title + "_Updated",
            content: question.Content + "_Updated",
            slug: question.Slug?.Value
        );

        var response = await DoPut("/question", request);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }
}