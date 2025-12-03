using BuildingBlocks.Logging;
using BuildingBlocks.Messaging;
using BuildingBlocks.Messaging.DomainEvents;
using BuildingBlocks.Messaging.IntegrationEvents.Interfaces;
using Forum.UnitTests.Fixtures;
using Microsoft.Extensions.DependencyInjection;

namespace Forum.UnitTests.Base;

public abstract class BaseTest(TestFixture fixture) : IClassFixture<TestFixture>, IDisposable
{
    #region Fields and Properties

    private readonly IServiceScope _scope = fixture.Services.CreateScope();

    protected TestFixture Fixture => fixture;
    protected ForumDbContext Context => _scope.ServiceProvider.GetRequiredService<ForumDbContext>();
    protected IDomainEventDispatcher DomainEventDispatcher => _scope.ServiceProvider.GetRequiredService<IDomainEventDispatcher>();
    protected HttpClient HttpClient { get; } = fixture.CreateClient();

    #endregion

    #region Domain Event Helpers

    protected TestDomainEventDispatcher GetTestDispatcher()
    {
        var dispatcher = DomainEventDispatcher as TestDomainEventDispatcher;
        if (dispatcher == null)
            throw new InvalidOperationException("DomainEventDispatcher is not a TestDomainEventDispatcher");
        return dispatcher;
    }

    protected void ClearDomainEvents()
    {
        var dispatcher = DomainEventDispatcher as TestDomainEventDispatcher;
        dispatcher?.Clear();
    }

    #endregion

    #region Mock Helpers

    protected static Mock<IAppLogger<T>> CreateLoggerMock<T>() => new();

    #endregion

    #region HTTP Request Helpers

    protected async Task<HttpResponseMessage> DoPost(string method, object request, string token = "",
        string culture = "en-US")
        => await fixture.DoPost(method, request, token, culture);

    protected async Task<HttpResponseMessage> DoGet(string method, string token = "", string culture = "en-US")
        => await fixture.DoGet(method, token, culture);

    protected async Task<HttpResponseMessage> DoPut(string method, object request, string token = "",
        string culture = "en-US")
        => await fixture.DoPut(method, request, token, culture);

    protected async Task<HttpResponseMessage> DoPatch(string method, object request, string token = "",
        string culture = "en-US")
        => await fixture.DoPatch(method, request, token, culture);

    protected async Task<HttpResponseMessage> DoDelete(string method, string token = "", string culture = "en-US")
        => await fixture.DoDelete(method, token, culture);

    #endregion

    #region Lifecycle

    public void Dispose()
    {
        _scope?.Dispose();
        GC.SuppressFinalize(this);
    }

    #endregion

    #region Test Helpers

    /// <summary>
    /// Helper class to track published integration events in tests.
    /// Use this to verify that integration events are published correctly without actual event bus infrastructure.
    /// </summary>
    protected class TestEventBus : IEventBus
    {
        public List<IIntegrationEvent> PublishedEvents { get; } = new();
        public bool ShouldThrowOnPublish { get; set; }

        public Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
            where TEvent : class, IIntegrationEvent
        {
            if (ShouldThrowOnPublish)
                throw new Exception("Event bus error");

            PublishedEvents.Add(@event);
            return Task.CompletedTask;
        }

        public void Subscribe<TEvent, THandler>()
            where TEvent : class, IIntegrationEvent
            where THandler : IIntegrationEventHandler<TEvent>
        {
            // Not needed for tests
        }

        public void Clear() => PublishedEvents.Clear();

        public TEvent? GetEvent<TEvent>() where TEvent : class, IIntegrationEvent
            => PublishedEvents.OfType<TEvent>().FirstOrDefault();

        public IEnumerable<TEvent> GetEvents<TEvent>() where TEvent : class, IIntegrationEvent
            => PublishedEvents.OfType<TEvent>();
    }

    #endregion
}