using BuildingBlocks.Base;
using BuildingBlocks.Logging;
using BuildingBlocks.Messaging;
using BuildingBlocks.Messaging.IntegrationEvents;
using Forum.Application.EventHandlers;
using Forum.Domain.Events;
using Forum.Domain.Repositories;
using Forum.UnitTests.Factories;

namespace Forum.UnitTests.Application.EventHandlers;

public class OnAnswerCreatedEventHandlerUnitTest
{
    private readonly Mock<IAppLogger<OnAnswerCreatedEventHandler>> _loggerMock;
    private readonly Mock<IEventBus> _eventBusMock;
    private readonly Mock<IAnswerRepository> _answerRepositoryMock;
    private readonly OnAnswerCreatedEventHandler _handler;

    public OnAnswerCreatedEventHandlerUnitTest()
    {
        _loggerMock = new Mock<IAppLogger<OnAnswerCreatedEventHandler>>();
        _eventBusMock = new Mock<IEventBus>();
        _answerRepositoryMock = new Mock<IAnswerRepository>();
        _handler = new OnAnswerCreatedEventHandler(
            _loggerMock.Object,
            _eventBusMock.Object,
            _answerRepositoryMock.Object
        );
    }

    [Fact]
    public async Task HandleAsync_ShouldPublishIntegrationEvent_WhenAnswerExists()
    {
        var answer = MakeAnswer.Create();
        var domainEvent = new AnswerCreatedEvent(answer);

        _answerRepositoryMock
            .Setup(x => x.FindById(answer.Id, false))
            .ReturnsAsync(answer);

        await _handler.HandleAsync(domainEvent, CancellationToken.None);

        _answerRepositoryMock.Verify(x => x.FindById(answer.Id, false), Times.Once);
        
        _eventBusMock.Verify(
            x => x.PublishAsync(
                It.Is<AnswerCreatedIntegrationEvent>(e =>
                    e.AnswerId == answer.Id.ToString() &&
                    e.QuestionId == answer.QuestionId.ToString() &&
                    e.AuthorId == answer.AuthorId.ToString() &&
                    e.Content == answer.Content
                ),
                CancellationToken.None
            ),
            Times.Once
        );

        _loggerMock.Verify(
            x => x.LogInformation(
                LogType.Functional,
                It.IsAny<string>(),
                It.IsAny<object[]>()
            ),
            Times.AtLeastOnce
        );
    }

    [Fact]
    public async Task HandleAsync_ShouldLogWarning_WhenAnswerNotFound()
    {
        var answerId = new UniqueEntityId();
        var answer = MakeAnswer.Create();
        answer.GetType().GetProperty("Id")!.SetValue(answer, answerId);
        var domainEvent = new AnswerCreatedEvent(answer);

        _answerRepositoryMock
            .Setup(x => x.FindById(answerId, false))
            .ReturnsAsync((AnswerEntity?)null);

        await _handler.HandleAsync(domainEvent, CancellationToken.None);

        _answerRepositoryMock.Verify(x => x.FindById(answerId, false), Times.Once);
        
        _eventBusMock.Verify(
            x => x.PublishAsync(It.IsAny<AnswerCreatedIntegrationEvent>(), It.IsAny<CancellationToken>()),
            Times.Never
        );

        _loggerMock.Verify(
            x => x.LogWarning(
                LogType.Functional,
                It.IsAny<string>(),
                It.IsAny<object[]>()
            ),
            Times.Once
        );
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowException_WhenRepositoryThrows()
    {
        var answer = MakeAnswer.Create();
        var domainEvent = new AnswerCreatedEvent(answer);
        var expectedException = new Exception("Repository error");

        _answerRepositoryMock
            .Setup(x => x.FindById(answer.Id, false))
            .ThrowsAsync(expectedException);

        var exception = await Assert.ThrowsAsync<Exception>(
            async () => await _handler.HandleAsync(domainEvent, CancellationToken.None)
        );

        Assert.Equal("Repository error", exception.Message);
        
        _loggerMock.Verify(
            x => x.LogError(
                LogType.Exception,
                It.IsAny<Exception>(),
                It.IsAny<string>(),
                It.IsAny<object[]>()
            ),
            Times.Once
        );
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowException_WhenEventBusThrows()
    {
        var answer = MakeAnswer.Create();
        var domainEvent = new AnswerCreatedEvent(answer);
        var expectedException = new Exception("Event bus error");

        _answerRepositoryMock
            .Setup(x => x.FindById(answer.Id, false))
            .ReturnsAsync(answer);

        _eventBusMock
            .Setup(x => x.PublishAsync(It.IsAny<AnswerCreatedIntegrationEvent>(), CancellationToken.None))
            .ThrowsAsync(expectedException);

        var exception = await Assert.ThrowsAsync<Exception>(
            async () => await _handler.HandleAsync(domainEvent, CancellationToken.None)
        );

        Assert.Equal("Event bus error", exception.Message);
        
        _loggerMock.Verify(
            x => x.LogError(
                LogType.Exception,
                It.IsAny<Exception>(),
                It.IsAny<string>(),
                It.IsAny<object[]>()
            ),
            Times.Once
        );
    }

    [Fact]
    public async Task HandleAsync_ShouldLogInformationMessages_WhenSuccessful()
    {
        var answer = MakeAnswer.Create();
        var domainEvent = new AnswerCreatedEvent(answer);

        _answerRepositoryMock
            .Setup(x => x.FindById(answer.Id, false))
            .ReturnsAsync(answer);

        await _handler.HandleAsync(domainEvent, CancellationToken.None);

        _loggerMock.Verify(
            x => x.LogInformation(
                LogType.Functional,
                "Handling AnswerCreatedEvent - AggregateId: {AggregateId}, OccurredAt: {OccurredAt}",
                It.IsAny<object[]>()
            ),
            Times.Once
        );

        _loggerMock.Verify(
            x => x.LogInformation(
                LogType.Functional,
                "Published AnswerCreatedIntegrationEvent - AnswerId: {AnswerId}",
                It.IsAny<object[]>()
            ),
            Times.Once
        );
    }
}