using BuildingBlocks.Base;
using BuildingBlocks.Logging;
using Forum.Domain.Repositories;

namespace Forum.Application.UseCases.Question.GetQuestions;

public record GetQuestionsResult(IEnumerable<QuestionEntity> Questions);

public interface IGetQuestionsUseCase
{
    Task<Result<GetQuestionsResult>> GetQuestionsUseCaseHandler();
}

public sealed class GetQuestionsUseCase(
    IAppLogger<GetQuestionsUseCase> logger,
    IQuestionRepository questionRepository)
    : IGetQuestionsUseCase
{
    public async Task<Result<GetQuestionsResult>> GetQuestionsUseCaseHandler()
    {
        var questions = await questionRepository.GetAll();
        return Result<GetQuestionsResult>.Success(new GetQuestionsResult(questions));
    }
}
