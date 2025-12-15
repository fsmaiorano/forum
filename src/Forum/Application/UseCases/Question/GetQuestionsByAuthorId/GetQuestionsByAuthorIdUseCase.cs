using BuildingBlocks.Base;
using BuildingBlocks.Logging;
using Forum.Application.UseCases.Question.GetQuestionById;
using Forum.Domain.Repositories;

namespace Forum.Application.UseCases.Question.GetQuestionsByAuthorId;

public record GetQuestionsByAuthorIdQuery(UniqueEntityId AuthorId);

public record GetQuestionsByAuthorIdResult(IEnumerable<QuestionEntity> Questions);

public interface IGetQuestionsByAuthorIdUseCase
{
    Task<Result<GetQuestionsByAuthorIdResult>> GetQuestionsByAuthorIdUseCaseHandler(GetQuestionsByAuthorIdQuery query);
}

public sealed class GetQuestionsByAuthorIdUseCase(
    IAppLogger<GetQuestionsByAuthorIdUseCase> logger,
    IQuestionRepository questionRepository,
    IAttachmentRepository attachmentRepository) : IGetQuestionsByAuthorIdUseCase
{
    public async Task<Result<GetQuestionsByAuthorIdResult>> GetQuestionsByAuthorIdUseCaseHandler(GetQuestionsByAuthorIdQuery query)
    {
        var questions = await questionRepository.GetByAuthor(query.AuthorId);
        return Result<GetQuestionsByAuthorIdResult>.Success(new GetQuestionsByAuthorIdResult(questions));
    }
}