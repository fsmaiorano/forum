using BuildingBlocks.Messaging.DomainEvents.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Messaging.DomainEvents;

/// <summary>
/// Service for dispatching domain events to registered handlers
/// </summary>
public interface IDomainEventDispatcher
{
    Task DispatchAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default);
    Task DispatchAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default);
}

/// <summary>
/// Default implementation of domain event dispatcher using service provider
/// </summary>
public class DomainEventDispatcher(IServiceProvider serviceProvider) : IDomainEventDispatcher
{
    public async Task DispatchAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        var eventType = domainEvent.GetType();
        var handlerType = typeof(IDomainEventHandler<>).MakeGenericType(eventType);

        using var scope = serviceProvider.CreateScope();
        var handlers = scope.ServiceProvider.GetServices(handlerType);

        var tasks = new List<Task>();
        foreach (var handler in handlers)
        {
            var method = handlerType.GetMethod(nameof(IDomainEventHandler<IDomainEvent>.HandleAsync));
            var task = (Task?)method?.Invoke(handler, [domainEvent, cancellationToken]);
            if (task != null)
                tasks.Add(task);
        }

        if (tasks.Count > 0)
            await Task.WhenAll(tasks);
    }

    public async Task DispatchAsync(IEnumerable<IDomainEvent> domainEvents,
        CancellationToken cancellationToken = default)
    {
        foreach (var domainEvent in domainEvents)
            await DispatchAsync(domainEvent, cancellationToken);
    }
}