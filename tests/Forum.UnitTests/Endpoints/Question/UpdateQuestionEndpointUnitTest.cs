using System.Net;
using Forum.UnitTests.Factories;
using Forum.UnitTests.Fixtures;

namespace Forum.UnitTests.Endpoints.Question;

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