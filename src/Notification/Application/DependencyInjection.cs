using BuildingBlocks.Exceptions;
using BuildingBlocks.Logging;
using BuildingBlocks.Messaging;
using BuildingBlocks.Messaging.Events;
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
        // Register subscribers
        services.AddScoped<IEventHandler<AnswerCreatedIntegrationEvent>, OnAnswerCreatedSubscriber>();
        services.AddScoped<IEventHandler<QuestionBestAnswerChosenIntegrationEvent>, OnQuestionBestAnswerChosenSubscriber>();

        // Setup subscriptions
        var serviceProvider = services.BuildServiceProvider();
        var eventBus = serviceProvider.GetRequiredService<IEventBus>();
        
        eventBus.Subscribe<AnswerCreatedIntegrationEvent, OnAnswerCreatedSubscriber>();
        eventBus.Subscribe<QuestionBestAnswerChosenIntegrationEvent, OnQuestionBestAnswerChosenSubscriber>();
    }
}