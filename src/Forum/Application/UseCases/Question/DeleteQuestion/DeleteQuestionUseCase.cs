using BuildingBlocks.Base;
using BuildingBlocks.Exceptions;
using BuildingBlocks.Logging;
using Forum.Domain.Repositories;

namespace Forum.Application.UseCases.Question.DeleteQuestion;

public record DeleteQuestionCommand(UniqueEntityId QuestionId);

public record DeleteQuestionResult();

public interface IDeleteQuestionUseCase
{
    Task<Result<DeleteQuestionResult>> DeleteQuestionUseCaseHandler(DeleteQuestionCommand command);
}

public class DeleteQuestionUseCase(
    IAppLogger<DeleteQuestionUseCase> logger,
    IQuestionRepository questionRepository,
    IAttachmentRepository attachmentRepository) : IDeleteQuestionUseCase
{
    public async Task<Result<DeleteQuestionResult>> DeleteQuestionUseCaseHandler(DeleteQuestionCommand command)
    {
        var question = await questionRepository.FindById(command.QuestionId) ??
                             throw new NotFoundException($"Question with ID {command.QuestionId} not found.");

        await attachmentRepository.DeleteByOwnerId(question.Id);
        await questionRepository.Delete(question);

        logger.LogInformation(LogType.Functional, $"Question with ID {command.QuestionId} deleted successfully.");

        return Result<DeleteQuestionResult>.Success(new DeleteQuestionResult());
    }
}