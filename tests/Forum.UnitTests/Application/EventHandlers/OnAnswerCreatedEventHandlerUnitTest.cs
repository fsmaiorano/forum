using BuildingBlocks.Base;
using BuildingBlocks.Messaging.IntegrationEvents;
using Forum.Application.EventHandlers;
using Forum.Domain.Events;
using Forum.UnitTests.Factories;
using Forum.UnitTests.Fixtures;

namespace Forum.UnitTests.Application.EventHandlers;

public class OnAnswerCreatedEventHandlerUnitTest(TestFixture fixture) : BaseTest(fixture)
{
    private readonly EventHandlerTestFixture<OnAnswerCreatedEventHandler> _eventHandlerFixture =
        new(fixture);

    [Fact]
    public async Task HandleAsync_ShouldPublishIntegrationEvent_WhenAnswerExists()
    {
        var answer = MakeAnswer.Create();
        await Context.Answer.AddAsync(answer);
        await Context.SaveChangesAsync();

        var domainEvent = new AnswerCreatedEvent(answer);
        var handler = _eventHandlerFixture.CreateHandler();

        await handler.HandleAsync(domainEvent, CancellationToken.None);

        Assert.Single(_eventHandlerFixture.EventBus.PublishedEvents);
        var integrationEvent = _eventHandlerFixture.GetEvent<AnswerCreatedIntegrationEvent>();

        Assert.NotNull(integrationEvent);
        Assert.Equal(answer.Id.ToString(), integrationEvent.AnswerId);
        Assert.Equal(answer.QuestionId.ToString(), integrationEvent.QuestionId);
        Assert.Equal(answer.AuthorId.ToString(), integrationEvent.AuthorId);
        Assert.Equal(answer.Content, integrationEvent.Content);
    }

    [Fact]
    public async Task HandleAsync_ShouldNotPublishIntegrationEvent_WhenAnswerNotFound()
    {
        var answer = MakeAnswer.Create();
        var domainEvent = new AnswerCreatedEvent(answer);
        var handler = _eventHandlerFixture.CreateHandler();

        await handler.HandleAsync(domainEvent, CancellationToken.None);

        Assert.Empty(_eventHandlerFixture.EventBus.PublishedEvents);
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowException_WhenEventBusThrows()
    {
        var answer = MakeAnswer.Create();
        await Context.Answer.AddAsync(answer);
        await Context.SaveChangesAsync();

        var domainEvent = new AnswerCreatedEvent(answer);
        _eventHandlerFixture.EventBus.ShouldThrowOnPublish = true;
        var handler = _eventHandlerFixture.CreateHandler();

        var exception =
            await Assert.ThrowsAsync<Exception>(async () =>
                await handler.HandleAsync(domainEvent, CancellationToken.None)
            );

        Assert.Equal("Event bus error", exception.Message);
    }

    [Fact]
    public async Task HandleAsync_ShouldPublishEventWithCorrectProperties_WhenSuccessful()
    {
        var answer = MakeAnswer.Create(
            authorId: new UniqueEntityId(),
            questionId: new UniqueEntityId(),
            content: "This is a test answer with specific content"
        );
        await Context.Answer.AddAsync(answer);
        await Context.SaveChangesAsync();

        var domainEvent = new AnswerCreatedEvent(answer);
        var handler = _eventHandlerFixture.CreateHandler();

        await handler.HandleAsync(domainEvent, CancellationToken.None);

        var integrationEvent = _eventHandlerFixture.GetEvent<AnswerCreatedIntegrationEvent>();
        Assert.NotNull(integrationEvent);
        Assert.NotEqual(Guid.Empty, integrationEvent.EventId);
        Assert.True(integrationEvent.OccurredAt <= DateTime.UtcNow);
        Assert.True(integrationEvent.OccurredAt >= DateTime.UtcNow.AddSeconds(-5));
    }

    [Fact]
    public async Task HandleAsync_ShouldHandleMultipleAnswers_WhenCalledSequentially()
    {
        var answer1 = MakeAnswer.Create();
        var answer2 = MakeAnswer.Create();
        var answer3 = MakeAnswer.Create();

        await Context.Answer.AddRangeAsync(answer1, answer2, answer3);
        await Context.SaveChangesAsync();

        var handler = _eventHandlerFixture.CreateHandler();

        await handler.HandleAsync(new AnswerCreatedEvent(answer1), CancellationToken.None);
        await handler.HandleAsync(new AnswerCreatedEvent(answer2), CancellationToken.None);
        await handler.HandleAsync(new AnswerCreatedEvent(answer3), CancellationToken.None);

        Assert.Equal(3, _eventHandlerFixture.EventBus.PublishedEvents.Count);

        var events = _eventHandlerFixture.GetEvents<AnswerCreatedIntegrationEvent>().ToList();
        Assert.Contains(events, e => e.AnswerId == answer1.Id.ToString());
        Assert.Contains(events, e => e.AnswerId == answer2.Id.ToString());
        Assert.Contains(events, e => e.AnswerId == answer3.Id.ToString());
    }
}