using System.Net;
using UnitTests.Factories;

namespace UnitTests.Endpoints.Question;

public class DeleteQuestionEndpointUnitTest(TestFixture fixture) : BaseTest(fixture)
{
    [Fact]
    public async Task DeleteQuestionEndpoint_ShouldReturn204()
    {
        var repository = new QuestionRepository(Context);

        var question = MakeQuestion.Create();
        await repository.Create(question);

        var response = await DoDelete($"/question/{question.Id}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }
}