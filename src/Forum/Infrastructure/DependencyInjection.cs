using Forum.Domain.Repositories;
using Forum.Infrastructure.Data.Context;
using Forum.Infrastructure.Data.Repositories;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Serilog;

namespace Forum.Infrastructure;

public static partial class DependencyInjection
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
        services.AddTransient<IQuestionRepository, QuestionRepository>();
    }

    private static void AddContexts(IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ForumDbContext>((serviceProvider, options) => { });

        services.AddHttpContextAccessor();
        services.AddScoped<IForumDbContext, ForumDbContext>();
    }

    private static WebApplicationBuilder AddLogging(WebApplicationBuilder builder)
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
            .WriteTo.Console());

        return builder;
    }
}