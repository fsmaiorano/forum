using BuildingBlocks.Base;
using BuildingBlocks.Logging;
using Forum.Domain.Repositories;

namespace Forum.Application.UseCases.Answer.GetAnswers;

public record GetAnswersResult(IEnumerable<AnswerEntity> Answers);

public interface IGetAnswersUseCase
{
    Task<Result<GetAnswersResult>> GetAnswersUseCaseHandler(string questionId);
}

public sealed class GetAnswersUseCase(
    IAppLogger<GetAnswersUseCase> logger,
    IAnswerRepository answerRepository)
    : IGetAnswersUseCase
{
    public async Task<Result<GetAnswersResult>> GetAnswersUseCaseHandler(string questionId)
    {
        var questionIdEntity = new UniqueEntityId(questionId);
        var answers = await answerRepository.GetByQuestionId(questionIdEntity);
        logger.LogInformation(LogTypeEnum.Functional, $"Retrieved {answers?.Count()} answers for question {questionId}");

        return Result<GetAnswersResult>.Success(new GetAnswersResult(answers));
    }
}
