using Forum.Application.UseCases.Question.CreateQuestion;

namespace Forum.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        AddUseCases(services);

        return services;
    }

    private static void AddUseCases(IServiceCollection services)
    {
        services.AddTransient<ICreateQuestionUseCase, CreateQuestionUseCase>();
    }
}