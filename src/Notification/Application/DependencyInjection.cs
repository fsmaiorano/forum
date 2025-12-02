using BuildingBlocks.Exceptions;
using BuildingBlocks.Logging;
using Notification.Application.UseCases.ReadNotification;
using Notification.Application.UseCases.SendNotification;

namespace Notification.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddSingleton(typeof(IAppLogger<>), typeof(AppLogger<>));
        
        services.AddExceptionHandler<CustomExceptionHandler>();
        services.AddProblemDetails();
        
        AddUseCases(services);

        return services;
    }

    private static void AddUseCases(IServiceCollection services)
    {
        services.AddTransient<ISendNotificationUseCase, SendNotificationUseCase>();
        services.AddTransient<IReadNotificationUseCase, ReadNotificationUseCase>();
    }
}