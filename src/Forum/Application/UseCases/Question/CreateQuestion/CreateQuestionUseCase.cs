using Forum.BuildingBlocks.Base;
using Forum.BuildingBlocks.Logging;
using Forum.Domain.Repositories;

namespace Forum.Application.UseCases.Question.CreateQuestion;

public record CreateQuestionCommand(string Title, string Content, string AuthorId, string? Slug = null);

public record CreateQuestionResult(string QuestionId);

public interface ICreateQuestionUseCase
{
    Task<Result<CreateQuestionResult>> CreateQuestionUseCaseHandler(CreateQuestionCommand command);
}

public sealed class CreateQuestionUseCase(IAppLogger<CreateQuestionUseCase> logger, IQuestionRepository questionRepository)
    : ICreateQuestionUseCase
{
    public async Task<Result<CreateQuestionResult>> CreateQuestionUseCaseHandler(CreateQuestionCommand command)
    {
        logger.LogInformation(LogType.Application, "Creating question with title: {Title}", command.Title);
        
        var question =
            Domain.Entities.Question.Create(
                command.AuthorId,
                command.Title,
                command.Content,
                command.Slug);

        await questionRepository.Create(question);
        
        logger.LogInformation(LogType.Application, "Question created with ID: {QuestionId}", question.Id);

        return Result<CreateQuestionResult>.Success(new CreateQuestionResult(question.Id.ToString()));
    }
}

