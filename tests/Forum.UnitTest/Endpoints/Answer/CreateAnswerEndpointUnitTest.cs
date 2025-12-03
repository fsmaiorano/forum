using System.Net;
using Forum.UnitTest.Base;
using Forum.UnitTest.Factories;
using Forum.UnitTest.Fixtures;

namespace Forum.UnitTest.Endpoints.Answer;

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