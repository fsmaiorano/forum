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
    IQuestionRepository questionRepository,
    IAttachmentRepository attachmentRepository)
    : IGetQuestionsUseCase
{
    public async Task<Result<GetQuestionsResult>> GetQuestionsUseCaseHandler()
    {
        var questions = await questionRepository.GetAll();

        foreach (var question in questions)
        {
            var attachments = await attachmentRepository.FindByOwnerId(question.Id);
            if (attachments.Count == 0) continue;
            question.Attachments.AddRange(attachments);
        }

        return Result<GetQuestionsResult>.Success(new GetQuestionsResult(questions));
    }
}