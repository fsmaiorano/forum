namespace Notification.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration,
        WebApplicationBuilder builder)
    {
        return services;
    }
}