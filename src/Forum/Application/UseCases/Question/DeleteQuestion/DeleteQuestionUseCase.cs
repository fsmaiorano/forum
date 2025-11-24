using Forum.BuildingBlocks.Base;
using Forum.BuildingBlocks.Exceptions;
using Forum.BuildingBlocks.Logging;
using Forum.Domain.Repositories;

namespace Forum.Application.UseCases.Question.DeleteQuestion;

public record DeleteQuestionRequest(string QuestionId);

public record DeleteQuestionResult();

public interface IDeleteQuestionUseCase
{
    Task<Result<DeleteQuestionResult>> DeleteQuestionUseCaseHandler(DeleteQuestionRequest request);
}

public class DeleteQuestionUseCase(
    IAppLogger<DeleteQuestionUseCase> logger,
    IQuestionRepository questionRepository,
    IAttachmentRepository attachmentRepository) : IDeleteQuestionUseCase
{
    public async Task<Result<DeleteQuestionResult>> DeleteQuestionUseCaseHandler(DeleteQuestionRequest request)
    {
        var question = await questionRepository.FindById(request.QuestionId) ??
                             throw new NotFoundException($"Question with ID {request.QuestionId} not found.");

        await attachmentRepository.DeleteByQuestionId(question.Id);
        await questionRepository.Delete(question);

        logger.LogInformation(LogType.Functional, $"Question with ID {request.QuestionId} deleted successfully.");

        return Result<DeleteQuestionResult>.Success(new DeleteQuestionResult());
    }
}