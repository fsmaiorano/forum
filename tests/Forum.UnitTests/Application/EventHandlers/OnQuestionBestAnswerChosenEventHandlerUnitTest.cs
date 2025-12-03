using BuildingBlocks.Messaging.IntegrationEvents;
using Forum.Application.EventHandlers;
using Forum.Domain.Events;
using Forum.UnitTests.Factories;
using Forum.UnitTests.Fixtures;

namespace Forum.UnitTests.Application.EventHandlers;

public class OnQuestionBestAnswerChosenEventHandlerUnitTest(TestFixture fixture) : BaseTest(fixture)
{
    private readonly EventHandlerTestFixture<OnQuestionBestAnswerChosenEventHandler> _eventHandlerFixture =
        new(fixture);

    [Fact]
    public async Task HandleAsync_ShouldPublishIntegrationEvent_WhenAnBestAnswerChosen()
    {
        var question = MakeQuestion.Create();
        await Context.Question.AddAsync(question);
        await Context.SaveChangesAsync();

        var answer = MakeAnswer.Create(question.Id);
        await Context.Answer.AddAsync(answer);
        await Context.SaveChangesAsync();

        QuestionEntity.SelectBestAnswer(question, answer.Id);
        await Context.SaveChangesAsync();

        var domainEvent = new QuestionBestAnswerChosenEvent(question, answer.Id);
        var handler = _eventHandlerFixture.CreateHandler();

        await handler.HandleAsync(domainEvent, CancellationToken.None);

        Assert.Single(_eventHandlerFixture.EventBus.PublishedEvents);
        var integrationEvent = _eventHandlerFixture.GetEvent<QuestionBestAnswerChosenIntegrationEvent>();

        Assert.NotNull(integrationEvent);
        Assert.Equal(answer.Id.ToString(), integrationEvent.BestAnswerId);
    }
}