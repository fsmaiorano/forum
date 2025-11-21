using Forum.Application.UseCases.Question.CreateQuestion;
using Forum.BuildingBlocks.Exceptions;
using Forum.BuildingBlocks.Logging;

namespace Forum.Application;

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
        services.AddTransient<ICreateQuestionUseCase, CreateQuestionUseCase>();
    }
}