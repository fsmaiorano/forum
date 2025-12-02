using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Messaging;

public class InMemoryEventBus(IServiceProvider serviceProvider) : IEventBus
{
    private readonly Dictionary<Type, List<Type>> _handlers = new();

    public async Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : class, IIntegrationEvent
    {
        var eventType = @event.GetType();

        if (!_handlers.TryGetValue(eventType, out var handlerTypes))
            return; // No handlers registered for this event

        using var scope = serviceProvider.CreateScope();
        foreach (var handlerType in handlerTypes)
        {
            var handler = scope.ServiceProvider.GetService(handlerType);
            if (handler is null) continue;

            var method = handlerType.GetMethod(nameof(IEventHandler<TEvent>.HandleAsync));

            var task = (Task?)method?.Invoke(handler, [@event, cancellationToken]);
            if (task != null)
                await task;
        }
    }

    public void Subscribe<TEvent, THandler>()
        where TEvent : class, IIntegrationEvent
        where THandler : IEventHandler<TEvent>
    {
        var eventType = typeof(TEvent);
        var handlerType = typeof(THandler);

        if (!_handlers.ContainsKey(eventType))
            _handlers[eventType] = new List<Type>();

        if (!_handlers[eventType].Contains(handlerType))
            _handlers[eventType].Add(handlerType);
    }
}