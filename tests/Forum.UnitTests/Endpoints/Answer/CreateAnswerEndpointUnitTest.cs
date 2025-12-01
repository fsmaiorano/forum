using System.Net;
using Forum.UnitTests.Factories;
using Forum.UnitTests.Fixtures;

namespace Forum.UnitTests.Endpoints.Answer;

public class CreateAnswerEndpointUnitTest(TestFixture fixture) : BaseTest(fixture)
{
    [Fact]
    public async Task CreateAnswerEndpoint_ShouldReturn201()
    {
        var question = MakeQuestion.Create();
        await Context.Question.AddAsync(question);
        await Context.SaveChangesAsync();

        var request = MakeAnswer.CreateAnswerRequest(question.Id.ToString());
        var response = await DoPost("/answer", request);
        
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }
}