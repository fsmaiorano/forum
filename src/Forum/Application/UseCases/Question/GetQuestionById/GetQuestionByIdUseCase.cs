using BuildingBlocks.Base;
using BuildingBlocks.Logging;
using Forum.Domain.Repositories;

namespace Forum.Application.UseCases.Question.GetQuestionById;

public record GetQuestionByIdQuery(UniqueEntityId QuestionId);

public record GetQuestionByIdResult(QuestionEntity? Question);

public interface IGetQuestionByIdUseCase
{
    Task<Result<GetQuestionByIdResult>> GetQuestionByIdUseCaseHandler(GetQuestionByIdQuery query);
}

public sealed class GetQuestionByIdUseCase(
    IAppLogger<GetQuestionByIdUseCase> logger,
    IQuestionRepository questionRepository,
    IAttachmentRepository attachmentRepository) : IGetQuestionByIdUseCase
{
    public async Task<Result<GetQuestionByIdResult>> GetQuestionByIdUseCaseHandler(GetQuestionByIdQuery query)
    {
        var question = await questionRepository.GetById(query.QuestionId);

        if (question is null) return Result<GetQuestionByIdResult>.Success(new GetQuestionByIdResult(question));

        var attachments = await attachmentRepository.FindByOwnerId(question.Id);
        question.Attachments.AddRange(attachments);

        return Result<GetQuestionByIdResult>.Success(new GetQuestionByIdResult(question));
    }
}