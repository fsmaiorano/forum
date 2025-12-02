namespace BuildingBlocks.Messaging;

/// <summary>
/// Interface for handling integration events
/// </summary>
public interface IEventHandler<in TEvent> where TEvent : IIntegrationEvent
{
    Task HandleAsync(TEvent @event, CancellationToken cancellationToken = default);
}