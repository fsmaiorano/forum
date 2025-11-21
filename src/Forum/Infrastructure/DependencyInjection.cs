using Forum.Domain.Repositories;
using Forum.Infrastructure.Data.Context;
using Forum.Infrastructure.Data.Repositories;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Forum.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
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
        var connectionString = configuration.GetConnectionString("ConnectionString:DefaultConnection")
                                  ?? throw new InvalidOperationException(
                                      "Database connection string is not configured.");
        ;

        services.AddScoped<ISaveChangesInterceptor>();

        services.AddDbContext<ForumDbContext>((serviceProvider, options) =>
        {
            options.AddInterceptors(serviceProvider.GetRequiredService<ISaveChangesInterceptor>());
            // options.UseNpgsql(connectionString);
        });

        services.AddHttpContextAccessor();
        services.AddScoped<IForumDbContext, ForumDbContext>();
    }
}