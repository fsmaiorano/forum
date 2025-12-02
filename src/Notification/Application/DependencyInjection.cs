using BuildingBlocks.Exceptions;
using BuildingBlocks.Logging;
using BuildingBlocks.Messaging;
using BuildingBlocks.Messaging.IntegrationEvents;
using BuildingBlocks.Messaging.IntegrationEvents.Interfaces;
using Notification.Application.HostedServices;
using Notification.Application.Subscribers;
using Notification.Application.UseCases.ReadNotification;
using Notification.Application.UseCases.SendNotification;

namespace Notification.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddSingleton(typeof(IAppLogger<>), typeof(AppLogger<>));
        services.AddSingleton<IEventBus, InMemoryEventBus>();
        
        services.AddExceptionHandler<CustomExceptionHandler>();
        services.AddProblemDetails();
        
        AddUseCases(services);
        AddSubscribers(services);

        return services;
    }

    private static void AddUseCases(IServiceCollection services)
    {
        services.AddTransient<ISendNotificationUseCase, SendNotificationUseCase>();
        services.AddTransient<IReadNotificationUseCase, ReadNotificationUseCase>();
    }

    private static void AddSubscribers(IServiceCollection services)
    {
        // Register integration event handlers
        services.AddScoped<IIntegrationEventHandler<AnswerCreatedIntegrationEvent>, OnAnswerCreatedSubscriber>();
        services.AddScoped<IIntegrationEventHandler<QuestionBestAnswerChosenIntegrationEvent>, OnQuestionBestAnswerChosenSubscriber>();

        // Setup subscriptions - this will be called after service provider is built
        // We need to subscribe after the application starts
        services.AddHostedService<IntegrationEventSubscriptionService>();
    }
}