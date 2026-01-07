using System.Text;
using System.Text.Json;
using BuildingBlocks.Messaging.IntegrationEvents.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace BuildingBlocks.Messaging;

/// <summary>
/// RabbitMQ implementation of event bus for distributed event-driven communication
/// </summary>
public class EventBus : IEventBus, IDisposable
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<EventBus> _logger;
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly Dictionary<Type, List<Type>> _handlers = new();
    private readonly string _exchangeName = "forum_events";

    public EventBus(IServiceProvider serviceProvider, ILogger<EventBus> logger, string rabbitMqConnectionString)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;

        var factory = new ConnectionFactory
        {
            Uri = new Uri(rabbitMqConnectionString),
            DispatchConsumersAsync = true
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        // Declare exchange
        _channel.ExchangeDeclare(_exchangeName, ExchangeType.Topic, durable: true, autoDelete: false);

        _logger.LogInformation("RabbitMQ EventBus initialized with exchange {ExchangeName}", _exchangeName);
    }

    public async Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : class, IIntegrationEvent
    {
        var eventType = @event.GetType();
        var routingKey = eventType.Name;

        _logger.LogInformation("Publishing integration event {EventType} with EventId {EventId}", 
            eventType.Name, @event.EventId);

        var message = JsonSerializer.Serialize(@event, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        var body = Encoding.UTF8.GetBytes(message);

        var properties = _channel.CreateBasicProperties();
        properties.ContentType = "application/json";
        properties.DeliveryMode = 2; // Persistent
        properties.MessageId = @event.EventId.ToString();
        properties.Timestamp = new AmqpTimestamp(new DateTimeOffset(@event.OccurredAt).ToUnixTimeSeconds());

        _channel.BasicPublish(
            exchange: _exchangeName,
            routingKey: routingKey,
            mandatory: false,
            basicProperties: properties,
            body: body);

        _logger.LogDebug("Published event {EventType} to RabbitMQ with routing key {RoutingKey}", 
            eventType.Name, routingKey);

        await Task.CompletedTask;
    }

    public void Subscribe<TEvent, THandler>()
        where TEvent : class, IIntegrationEvent
        where THandler : IIntegrationEventHandler<TEvent>
    {
        var eventType = typeof(TEvent);
        var handlerType = typeof(THandler);

        if (!_handlers.ContainsKey(eventType))
            _handlers[eventType] = [];

        if (!_handlers[eventType].Contains(handlerType))
        {
            _handlers[eventType].Add(handlerType);
            _logger.LogInformation("Subscribed {HandlerType} to {EventType}", handlerType.Name, eventType.Name);
        }

        // Create dead letter queue and exchange
        var queueName = $"queue_{eventType.Name}";
        var deadLetterQueueName = $"queue_{eventType.Name}_dlq";
        var deadLetterExchangeName = $"{_exchangeName}_dlq";
        var routingKey = eventType.Name;

        // Declare dead letter exchange
        _channel.ExchangeDeclare(deadLetterExchangeName, ExchangeType.Topic, durable: true, autoDelete: false);

        // Declare dead letter queue
        _channel.QueueDeclare(
            queue: deadLetterQueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        // Bind dead letter queue to dead letter exchange
        _channel.QueueBind(
            queue: deadLetterQueueName,
            exchange: deadLetterExchangeName,
            routingKey: routingKey);

        // Create main queue with dead letter configuration
        var queueArguments = new Dictionary<string, object>
        {
            { "x-dead-letter-exchange", deadLetterExchangeName },
            { "x-dead-letter-routing-key", routingKey }
        };

        _channel.QueueDeclare(
            queue: queueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: queueArguments);

        _channel.QueueBind(
            queue: queueName,
            exchange: _exchangeName,
            routingKey: routingKey);

        // Set up consumer
        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.Received += async (_, ea) =>
        {
            var deliveryTag = ea.DeliveryTag;
            
            try
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                _logger.LogDebug("Received event {EventType} from RabbitMQ", eventType.Name);

                var @event = JsonSerializer.Deserialize<TEvent>(message, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                if (@event != null)
                {
                    await ProcessEventAsync(@event, eventType, CancellationToken.None);
                    
                    // Acknowledge the message successfully processed
                    _channel.BasicAck(deliveryTag, false);
                    _logger.LogDebug("Message acknowledged for event {EventType}", eventType.Name);
                }
                else
                {
                    _logger.LogWarning("Failed to deserialize event {EventType}. Rejecting message to dead letter queue.", eventType.Name);
                    
                    // Reject message without requeue - will go to dead letter queue
                    _channel.BasicReject(deliveryTag, false);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing event {EventType}. Rejecting message to dead letter queue.", eventType.Name);
                
                // Reject message without requeue - will go to dead letter queue
                _channel.BasicReject(deliveryTag, false);
            }
        };

        _channel.BasicConsume(
            queue: queueName,
            autoAck: false,
            consumer: consumer);

        _logger.LogInformation("Started consuming messages from queue {QueueName} with dead letter queue {DeadLetterQueueName}", 
            queueName, deadLetterQueueName);
    }

    private async Task ProcessEventAsync<TEvent>(TEvent @event, Type eventType, CancellationToken cancellationToken)
        where TEvent : class, IIntegrationEvent
    {
        if (!_handlers.TryGetValue(eventType, out var handlerTypes))
        {
            _logger.LogDebug("No handlers registered for event type {EventType}", eventType.Name);
            return;
        }

        using var scope = _serviceProvider.CreateScope();
        var tasks = new List<Task>();

        foreach (var handlerType in handlerTypes)
        {
            var handler = scope.ServiceProvider.GetService(handlerType);
            if (handler is null)
            {
                _logger.LogWarning("Handler {HandlerType} not found in service provider", handlerType.Name);
                continue;
            }

            var method = handlerType.GetMethod(nameof(IIntegrationEventHandler<TEvent>.HandleAsync));
            var task = (Task?)method?.Invoke(handler, [@event, cancellationToken]);
            
            if (task != null)
                tasks.Add(task);
        }

        if (tasks.Count > 0)
            await Task.WhenAll(tasks);
    }

    public void Dispose()
    {
        _logger.LogInformation("Disposing RabbitMQ EventBus");
        _channel.Dispose();
        _connection.Dispose();
    }
}