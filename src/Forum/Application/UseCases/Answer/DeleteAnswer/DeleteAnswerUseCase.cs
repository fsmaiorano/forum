using Forum.BuildingBlocks.Base;
using Forum.BuildingBlocks.Exceptions;
using Forum.BuildingBlocks.Logging;
using Forum.Domain.Repositories;

namespace Forum.Application.UseCases.Answer.DeleteAnswer;

public record DeleteAnswerCommand(UniqueEntityId AnswerId);

public record DeleteAnswerResult();

public interface IDeleteAnswerUseCase
{
    Task<Result<DeleteAnswerResult>> DeleteAnswerUseCaseHandler(DeleteAnswerCommand command);
}

public class DeleteAnswerUseCase(
    IAppLogger<DeleteAnswerUseCase> logger,
    IAnswerRepository answerRepository,
    IAttachmentRepository attachmentRepository) : IDeleteAnswerUseCase
{
    public async Task<Result<DeleteAnswerResult>> DeleteAnswerUseCaseHandler(DeleteAnswerCommand command)
    {
        var answer = await answerRepository.FindById(command.AnswerId) ??
                             throw new NotFoundException($"Answer with ID {command.AnswerId} not found.");

        await attachmentRepository.DeleteByOwnerId(answer.Id);
        await answerRepository.Delete(answer);

        logger.LogInformation(LogType.Functional, $"Answer with ID {command.AnswerId} deleted successfully.");

        return Result<DeleteAnswerResult>.Success(new DeleteAnswerResult());
    }
}