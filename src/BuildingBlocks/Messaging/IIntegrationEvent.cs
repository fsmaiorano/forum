namespace BuildingBlocks.Messaging;

/// <summary>
/// Base interface for integration events (cross-service events)
/// </summary>
public interface IIntegrationEvent
{
    Guid EventId { get; }
    DateTime OccurredAt { get; }
}

