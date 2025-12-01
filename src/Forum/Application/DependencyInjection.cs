using BuildingBlocks.Exceptions;
using BuildingBlocks.Logging;
using Forum.Application.UseCases.Answer.CreateAnswer;
using Forum.Application.UseCases.Answer.DeleteAnswer;
using Forum.Application.UseCases.Answer.UpdateAnswer;
using Forum.Application.UseCases.Question.CreateQuestion;
using Forum.Application.UseCases.Question.DeleteQuestion;
using Forum.Application.UseCases.Question.PatchQuestionSetBestAnswer;
using Forum.Application.UseCases.Question.UpdateQuestion;

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
        services.AddTransient<IUpdateQuestionUseCase, UpdateQuestionUseCase>();
        services.AddTransient<IPatchQuestionSetBestAnswerUseCase, PatchQuestionSetBestAnswerUseCase>();
        services.AddTransient<IDeleteQuestionUseCase, DeleteQuestionUseCase>();

        services.AddTransient<ICreateAnswerUseCase, CreateAnswerUseCase>();
        services.AddTransient<IUpdateAnswerUseCase, UpdateAnswerUseCase>();
        services.AddTransient<IDeleteAnswerUseCase, DeleteAnswerUseCase>();
    }
}