using BuildingBlocks.Exceptions;
using BuildingBlocks.Logging;
using BuildingBlocks.Messaging;
using BuildingBlocks.Messaging.DomainEvents;
using BuildingBlocks.Messaging.DomainEvents.Interfaces;
using Forum.Application.EventHandlers;
using Forum.Application.UseCases.Answer.CreateAnswer;
using Forum.Application.UseCases.Answer.DeleteAnswer;
using Forum.Application.UseCases.Answer.GetAnswers;
using Forum.Application.UseCases.Answer.UpdateAnswer;
using Forum.Application.UseCases.Question.CreateQuestion;
using Forum.Application.UseCases.Question.DeleteQuestion;
using Forum.Application.UseCases.Question.GetQuestions;
using Forum.Application.UseCases.Question.PatchQuestionSetBestAnswer;
using Forum.Application.UseCases.Question.UpdateQuestion;
using Forum.Domain.Events;

namespace Forum.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddSingleton(typeof(IAppLogger<>), typeof(AppLogger<>));
        services.AddSingleton<IEventBus, InMemoryEventBus>();
        services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();

        services.AddExceptionHandler<CustomExceptionHandler>();
        services.AddProblemDetails();

        AddUseCases(services);
        AddDomainEventHandlers(services);

        return services;
    }

    private static void AddUseCases(IServiceCollection services)
    {
        services.AddTransient<ICreateQuestionUseCase, CreateQuestionUseCase>();
        services.AddTransient<IGetQuestionsUseCase, GetQuestionsUseCase>();
        services.AddTransient<IUpdateQuestionUseCase, UpdateQuestionUseCase>();
        services.AddTransient<IPatchQuestionSetBestAnswerUseCase, PatchQuestionSetBestAnswerUseCase>();
        services.AddTransient<IDeleteQuestionUseCase, DeleteQuestionUseCase>();

        services.AddTransient<ICreateAnswerUseCase, CreateAnswerUseCase>();
        services.AddTransient<IGetAnswersUseCase, GetAnswersUseCase>();
        services.AddTransient<IUpdateAnswerUseCase, UpdateAnswerUseCase>();
        services.AddTransient<IDeleteAnswerUseCase, DeleteAnswerUseCase>();
    }

    private static void AddDomainEventHandlers(IServiceCollection services)
    {
        services.AddScoped<IDomainEventHandler<AnswerCreatedEvent>, OnAnswerCreatedEventHandler>();
        services.AddScoped<IDomainEventHandler<QuestionBestAnswerChosenEvent>, OnQuestionBestAnswerChosenEventHandler>();
    }
}