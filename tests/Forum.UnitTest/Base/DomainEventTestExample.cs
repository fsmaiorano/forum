using BuildingBlocks.Messaging.DomainEvents;
using Forum.Domain.Events;
using Forum.UnitTest.Fixtures;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Forum.UnitTest.Base;

/// <summary>
/// Example test showing how to use TestDomainEventDispatcher
/// </summary>
public class DomainEventTestExample : IClassFixture<TestFixture>
{
    private readonly TestFixture _fixture;

    public DomainEventTestExample(TestFixture fixture)
    {
        _fixture = fixture;
    }

    /// <summary>
    /// Example: Verify that domain events are raised without triggering handlers
    /// </summary>
    [Fact]
    public async Task Example_VerifyDomainEventWasRaised()
    {
        // Arrange
        var dbContext = _fixture.GetDbContext();
        var dispatcher = _fixture.Services.GetRequiredService<IDomainEventDispatcher>() as TestDomainEventDispatcher;

        // Clear any previous events
        dispatcher?.Clear();

        // Act
        // ... perform some action that raises domain events ...

        // Assert
        Assert.NotNull(dispatcher);

        // Check if a specific event was raised
        var hasAnswerCreatedEvent = dispatcher.HasEvent<AnswerCreatedEvent>();

        // Get a specific event
        var answerCreatedEvent = dispatcher.GetEvent<AnswerCreatedEvent>();

        // Get all events of a type
        var allAnswerCreatedEvents = dispatcher.GetEvents<AnswerCreatedEvent>();

        // Assert based on your test needs
        // Assert.True(hasAnswerCreatedEvent);
        // Assert.NotNull(answerCreatedEvent);
    }
}