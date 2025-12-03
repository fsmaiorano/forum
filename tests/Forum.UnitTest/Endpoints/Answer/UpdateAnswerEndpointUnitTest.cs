using System.Net;
using Forum.UnitTest.Base;
using Forum.UnitTest.Factories;
using Forum.UnitTest.Fixtures;

namespace Forum.UnitTest.Endpoints.Answer;

public class UpdateAnswerEndpointUnitTest(TestFixture fixture)
    : BaseTest(fixture)
{
    [Fact]
    public async Task UpdateAnswerEndpoint_ShouldReturn204()
    {
        var repository = new AnswerRepository(Context, DomainEventDispatcher);

        var answer = MakeAnswer.Create();
        await repository.Create(answer);

        Thread.Sleep(1000);

        var request = MakeAnswer.UpdateAnswerRequest(
            answerId: answer.Id.ToString(),
            authorId: answer.AuthorId.ToString(),
            content: answer.Content + "_Updated"
        );

        var response = await DoPut("/answer", request);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }
}