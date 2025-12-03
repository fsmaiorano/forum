using System.Net;
using Forum.UnitTest.Base;
using Forum.UnitTest.Factories;
using Forum.UnitTest.Fixtures;

namespace Forum.UnitTest.Endpoints.Question;

public class DeleteQuestionEndpointUnitTest(TestFixture fixture) : BaseTest(fixture)
{
    [Fact]
    public async Task DeleteQuestionEndpoint_ShouldReturn204()
    {
        var repository = new QuestionRepository(Context, DomainEventDispatcher);

        var question = MakeQuestion.Create();
        await repository.Create(question);

        var response = await DoDelete($"/question/{question.Id}");

        var storedQuestion = await Context.Question.FirstOrDefaultAsync(q => q.Id.Equals(question.Id));
        var storedAttachments = await Context.Attachment.Where(a => a.OwnerId.Equals(question.Id)).ToListAsync();

        Assert.Null(storedQuestion);
        Assert.Empty(storedAttachments);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }
}