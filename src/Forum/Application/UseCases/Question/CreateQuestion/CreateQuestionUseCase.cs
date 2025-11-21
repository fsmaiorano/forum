using Forum.Domain.Base;
using Forum.Domain.Repositories;

namespace Forum.Application.UseCases.Question.CreateQuestion;

public record CreateQuestionCommand(string Title, string Content, string AuthorId, string? Slug = null);

public record CreateQuestionResult(string QuestionId);

public interface ICreateQuestionUseCase;

public sealed class CreateQuestionUseCase(ILogger<CreateQuestionUseCase> logger, IQuestionRepository questionRepository)
    : ICreateQuestionUseCase
{
    public async Task<Result<CreateQuestionResult>> CreateQuestionUseCaseHandler(CreateQuestionCommand command)
    {
        var question =
            Domain.Entities.Question.Create(
                command.AuthorId,
                command.Title,
                command.Content,
                command.Slug);

        await questionRepository.Create(question);

        return Result<CreateQuestionResult>.Success(new CreateQuestionResult(question.Id.ToString()));
    }
}