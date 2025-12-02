using BuildingBlocks.Exceptions;
using BuildingBlocks.Logging;
using BuildingBlocks.Messaging;
using Forum.Application.EventHandlers;
using Forum.Application.HostedServices;
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
        services.AddSingleton<IEventBus, InMemoryEventBus>();

        services.AddExceptionHandler<CustomExceptionHandler>();
        services.AddProblemDetails();

        AddUseCases(services);
        AddEventHandlers(services);

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

    private static void AddEventHandlers(IServiceCollection services)
    {
        services.AddSingleton<OnAnswerCreated>();
        services.AddSingleton<OnQuestionBestAnswerChosen>();

        services.AddHostedService<EventHandlerSubscriptionService>();
    }
}