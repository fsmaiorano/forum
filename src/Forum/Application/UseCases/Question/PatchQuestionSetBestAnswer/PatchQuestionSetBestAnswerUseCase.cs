using BuildingBlocks.Base;
using BuildingBlocks.Exceptions;
using BuildingBlocks.Logging;
using Forum.Domain.Repositories;

namespace Forum.Application.UseCases.Question.PatchQuestionSetBestAnswer;

public record PatchQuestionSetBestAnswerCommand(
    UniqueEntityId QuestionId,
    UniqueEntityId AnswerId);

public record PatchQuestionSetBestAnswerResult();

public interface IPatchQuestionSetBestAnswerUseCase
{
    Task<Result<PatchQuestionSetBestAnswerResult>> PatchQuestionSetBestAnswerUseCaseHandler(
        PatchQuestionSetBestAnswerCommand command);
}

public class PatchQuestionSetBestAnswerUseCase(
    IAppLogger<PatchQuestionSetBestAnswerUseCase> logger,
    IQuestionRepository questionRepository) : IPatchQuestionSetBestAnswerUseCase
{
    public async Task<Result<PatchQuestionSetBestAnswerResult>> PatchQuestionSetBestAnswerUseCaseHandler(
        PatchQuestionSetBestAnswerCommand command)
    {
        var question = await questionRepository.FindById(command.QuestionId);

        if (question is null)
            throw new NotFoundException(nameof(Question), command.QuestionId);
        
        QuestionEntity.SelectBestAnswer(question, command.AnswerId);
        await questionRepository.SelectBestAnswer(question);

        return Result<PatchQuestionSetBestAnswerResult>.Success(new PatchQuestionSetBestAnswerResult());
    }
}