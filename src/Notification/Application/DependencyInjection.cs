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
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton(typeof(IAppLogger<>), typeof(AppLogger<>));

        var rabbitMqConnectionString = configuration["RabbitMQ:ConnectionString"]
                                       ?? throw new InvalidOperationException(
                                           "RabbitMQ:ConnectionString is not configured");

        services.AddSingleton<IEventBus>(sp =>
        {
            var logger = sp.GetRequiredService<ILogger<EventBus>>();
            return new EventBus(sp, logger, rabbitMqConnectionString);
        });

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
        services.AddScoped<OnAnswerCreatedSubscriber>();
        services.AddScoped<OnQuestionBestAnswerChosenSubscriber>();
        services.AddHostedService<IntegrationEventSubscriptionService>();
    }
}