using BuildingBlocks.Messaging;
using BuildingBlocks.Messaging.IntegrationEvents;
using Notification.Application.Subscribers;

namespace Notification.Application.HostedServices;

/// <summary>
/// Hosted service that subscribes to integration events on application startup
/// </summary>
public class IntegrationEventSubscriptionService(IEventBus eventBus) : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        // Subscribe to integration events
        eventBus.Subscribe<AnswerCreatedIntegrationEvent, OnAnswerCreatedSubscriber>();
        eventBus.Subscribe<QuestionBestAnswerChosenIntegrationEvent, OnQuestionBestAnswerChosenSubscriber>();

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}

