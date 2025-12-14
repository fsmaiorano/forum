using System.Net;
using System.Text.Json;
using Forum.Endpoints.Answer;
using Forum.Endpoints.Question;
using Forum.UnitTest.Factories;
using Forum.UnitTest.Fixtures;

namespace Forum.UnitTest.Endpoints.Answer;

public class GetAnswersEndpointUnitTest(TestFixture fixture) : BaseTest(fixture)
{
    [Fact]
    public async Task GetQuestionEndpoint_ShouldReturn200AndQuestion()
    {
        var questionRepository = new QuestionRepository(Context, DomainEventDispatcher);
        var answerRepository = new AnswerRepository(Context, DomainEventDispatcher);

        var question = MakeQuestion.Create();
        await questionRepository.Create(question);

        var answer = MakeAnswer.Create(questionId: question.Id);
        await answerRepository.Create(answer);

        var response = await DoGet($"/answer/{question.Id}");

        var content = await response.Content.ReadAsStringAsync();

        var result = JsonSerializer.Deserialize<GetAnswersResponse>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(result);
        Assert.NotEmpty(result.Answers);
        Assert.Single(result.Answers);
    }
}