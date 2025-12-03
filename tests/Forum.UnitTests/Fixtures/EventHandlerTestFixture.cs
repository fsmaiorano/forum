using BuildingBlocks.Logging;
using BuildingBlocks.Messaging;
using BuildingBlocks.Messaging.IntegrationEvents.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Forum.UnitTests.Fixtures;

/// <summary>
/// Fixture for testing event handlers with real dependencies and test event bus
/// </summary>
/// <typeparam name="TEventHandler">The event handler type to create</typeparam>
public class EventHandlerTestFixture<TEventHandler> where TEventHandler : class
{
    private readonly TestFixture _testFixture;
    
    public TestEventBus EventBus { get; }
    
    public EventHandlerTestFixture(TestFixture testFixture)
    {
        _testFixture = testFixture;
        EventBus = new TestEventBus();
    }

    /// <summary>
    /// Create an instance of the event handler with real dependencies
    /// </summary>
    public TEventHandler CreateHandler(params object[] additionalDependencies)
    {
        var scope = _testFixture.Services.CreateScope();
        var serviceProvider = scope.ServiceProvider;
        
        // Get constructor parameters
        var constructor = typeof(TEventHandler).GetConstructors().First();
        var parameters = constructor.GetParameters();
        var arguments = new List<object>();

        foreach (var param in parameters)
        {
            var paramType = param.ParameterType;
            
            // Use TestEventBus for IEventBus
            if (paramType == typeof(IEventBus))
            {
                arguments.Add(EventBus);
            }
            // Create real logger
            else if (paramType.IsGenericType && paramType.GetGenericTypeDefinition() == typeof(IAppLogger<>))
            {
                var loggerType = typeof(ILogger<>).MakeGenericType(typeof(TEventHandler));
                var logger = serviceProvider.GetRequiredService(loggerType);
                var appLoggerType = typeof(AppLogger<>).MakeGenericType(typeof(TEventHandler));
                arguments.Add(Activator.CreateInstance(appLoggerType, logger)!);
            }
            // Resolve from DI
            else
            {
                var service = serviceProvider.GetRequiredService(paramType);
                arguments.Add(service);
            }
        }

        return (TEventHandler)Activator.CreateInstance(typeof(TEventHandler), arguments.ToArray())!;
    }

    /// <summary>
    /// Clear all published events from the event bus
    /// </summary>
    public void ClearEvents() => EventBus.Clear();

    /// <summary>
    /// Get the first published event of a specific type
    /// </summary>
    public TEvent? GetEvent<TEvent>() where TEvent : class, IIntegrationEvent
        => EventBus.GetEvent<TEvent>();

    /// <summary>
    /// Get all published events of a specific type
    /// </summary>
    public IEnumerable<TEvent> GetEvents<TEvent>() where TEvent : class, IIntegrationEvent
        => EventBus.GetEvents<TEvent>();

    /// <summary>
    /// Test implementation of IEventBus that captures published events
    /// </summary>
    public class TestEventBus : IEventBus
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
}

