using BuildingBlocks.Base;
using Forum.UnitTest.Factories;
using Forum.UnitTest.Fixtures;

namespace Forum.UnitTest.Endpoints.Question;

public class PatchQuestionSetBestAnswerEndpointUnitTest(TestFixture fixture)
    : BaseTest(fixture)
{
    [Fact]
    public async Task PatchQuestionSetBestAnswerEndpoint_ShouldReturn204()
    {
        var repository = new QuestionRepository(Context, DomainEventDispatcher);

        var question = MakeQuestion.Create();
        await repository.Create(question);

        var bestAnswerId = new UniqueEntityId();

        var request = MakeQuestion.PatchQuestionSetBestAnswerRequest(
            questionId: question.Id.ToString(),
            bestAnswerId: bestAnswerId.ToString()
        );

        var response = await DoPatch($"/question/{question.Id.ToString()}/best-answer", request);

        Assert.Equal(System.Net.HttpStatusCode.NoContent, response.StatusCode);
    }
}