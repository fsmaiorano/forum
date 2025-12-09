using BuildingBlocks.Base;
using BuildingBlocks.Logging;
using Forum.Domain.Repositories;

namespace Forum.Application.UseCases.Answer.GetAnswers;

public record GetAnswersQuery(string QuestionId);

public record GetAnswersResult(IEnumerable<AnswerEntity> Answers);

public interface IGetAnswersUseCase
{
    Task<Result<GetAnswersResult>> GetAnswersUseCaseHandler(GetAnswersQuery query);
}

public sealed class GetAnswersUseCase(
    IAppLogger<GetAnswersUseCase> logger,
    IAnswerRepository answerRepository)
    : IGetAnswersUseCase
{
    public async Task<Result<GetAnswersResult>> GetAnswersUseCaseHandler(GetAnswersQuery query)
    {
        var questionIdEntity = new UniqueEntityId(query.QuestionId);
        var answers = await answerRepository.GetByQuestionId(questionIdEntity);
        logger.LogInformation(LogTypeEnum.Functional, $"Retrieved {answers?.Count()} answers for question {questionIdEntity}");

        return Result<GetAnswersResult>.Success(new GetAnswersResult(answers));
    }
}
