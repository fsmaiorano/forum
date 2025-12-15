using System.Net;
using System.Text.Json;
using Forum.Endpoints.Question;
using Forum.UnitTest.Factories;
using Forum.UnitTest.Fixtures;

namespace Forum.UnitTest.Endpoints.Question;

public class GetQuestionsEndpointUnitTest(TestFixture fixture) : BaseTest(fixture)
{
    [Fact]
    public async Task GetQuestionEndpoint_ShouldReturn200AndQuestion()
    {
        var repository = new QuestionRepository(Context, DomainEventDispatcher);

        var question = MakeQuestion.Create();
        await repository.Create(question);

        var response = await DoGet($"/question/author/{question.Id}");

        var content = await response.Content.ReadAsStringAsync();

        var result = JsonSerializer.Deserialize<GetQuestionsResponse>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        });
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(result);
        Assert.NotEmpty(result.Questions);
        Assert.Single(result.Questions);
    }
}