using System.Net;
using Forum.UnitTests.Factories;
using Forum.UnitTests.Fixtures;

namespace Forum.UnitTests.Endpoints.Answer;

public class DeleteAnswerEndpointUnitTest(TestFixture fixture) : BaseTest(fixture)
{
    [Fact]
    public async Task DeleteAnswerEndpoint_ShouldReturn204()
    {
        var repository = new AnswerRepository(Context);

        var answer = MakeAnswer.Create();
        await repository.Create(answer);

        Thread.Sleep(1000);
        
        var response = await DoDelete($"/answer/{answer.Id}");

        var storedAnswer = await Context.Answer.FirstOrDefaultAsync(q => q.Id.Equals(answer.Id));
        var storedAttachments = await Context.Attachment.Where(a => a.OwnerId.Equals(answer.Id)).ToListAsync();

        Assert.Null(storedAnswer);
        Assert.Empty(storedAttachments);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }
}