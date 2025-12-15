using Microsoft.EntityFrameworkCore;
using Notification.Domain.Repositories;
using Notification.Infrastructure.Data.Context;
using Notification.Infrastructure.Data.Repositories;
using Serilog;

namespace Notification.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration,
        WebApplicationBuilder builder)
    {
        AddLogging(builder);
        AddContexts(services, configuration);
        AddRepositories(services);
        return services;
    }

    private static void AddRepositories(IServiceCollection services)
    {
        services.AddScoped<INotificationRepository, NotificationRepository>();
    }

    private static void AddContexts(IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<INotificationDbContext, NotificationDbContext>();

        if (configuration.GetSection("UseInMemoryDatabase").Get<bool>())
        {
            services.AddDbContext<NotificationDbContext>(options =>
                options.UseInMemoryDatabase("NotificationInMemoryDb"));
        }
        else
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<NotificationDbContext>((options) => { options.UseNpgsql(connectionString); });
        }
    }

    private static void AddLogging(WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog((context, services, configuration) => configuration
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext()
            .WriteTo.Logger(lc => lc
                .Filter.ByIncludingOnly("LogType = 'Application'")
                .WriteTo.File("Logs/Application/log-.txt",
                    rollingInterval: RollingInterval.Day,
                    outputTemplate:
                    "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] [{LogType}] {Message:lj}{NewLine}{Exception}"))
            .WriteTo.Logger(lc => lc
                .Filter.ByIncludingOnly("LogType = 'Functional'")
                .WriteTo.File("Logs/Functional/log-.txt",
                    rollingInterval: RollingInterval.Day,
                    outputTemplate:
                    "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] [{LogType}] {Message:lj}{NewLine}{Exception}"))
            .WriteTo.Logger(lc => lc
                .Filter.ByIncludingOnly("LogType = 'Exception'")
                .WriteTo.File("Logs/Exception/log-.txt",
                    rollingInterval: RollingInterval.Day,
                    outputTemplate:
                    "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] [{LogType}] {Message:lj}{NewLine}{Exception}"))
            .WriteTo.Console());
    }
}