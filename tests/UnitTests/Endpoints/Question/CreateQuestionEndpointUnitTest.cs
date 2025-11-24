using System.Net;
using UnitTests.Factories;

namespace UnitTests.Endpoints.Question;

public class CreateQuestionEndpointUnitTest(DatabaseFixture databaseFixture, HttpFixture httpFixture)
    : BaseTest(databaseFixture, httpFixture)
{
    [Fact]
    public async Task CreateQuestionEndpoint_ShouldReturn201()
    {
        var request = MakeQuestion.CreateQuestionRequest();
        var response = await DoPost("/question", request);
        
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }
}