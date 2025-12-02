namespace BuildingBlocks.Messaging;

/// <summary>
/// Interface for publishing and subscribing to events across services
/// </summary>
public interface IEventBus
{
    /// <summary>
    /// Publish an event to all subscribers
    /// </summary>
    Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) 
        where TEvent : class, IIntegrationEvent;
    
    /// <summary>
    /// Subscribe to events of a specific type
    /// </summary>
    void Subscribe<TEvent, THandler>()
        where TEvent : class, IIntegrationEvent
        where THandler : IEventHandler<TEvent>;
}

