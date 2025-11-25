using Forum.BuildingBlocks.Base;
using Forum.BuildingBlocks.Exceptions;
using Forum.BuildingBlocks.Logging;
using Forum.Domain.Enums;
using Forum.Domain.Repositories;

namespace Forum.Application.UseCases.Question.UpdateQuestion;

public record UpdateQuestionCommand(
    UniqueEntityId QuestionId,
    UniqueEntityId AuthorId,
    string Title,
    string Content,
    string? Slug = null,
    List<AttachmentEntity>? Attachments = null);

public record UpdateQuestionResult();

public interface IUpdateQuestionUseCase
{
    Task<Result<UpdateQuestionResult>> UpdateQuestionUseCaseHandler(UpdateQuestionCommand command);
}

public class UpdateQuestionUseCase(
    IAppLogger<UpdateQuestionUseCase> logger,
    IQuestionRepository questionRepository,
    IAttachmentRepository attachmentRepository)
    : IUpdateQuestionUseCase
{
    public async Task<Result<UpdateQuestionResult>> UpdateQuestionUseCaseHandler(UpdateQuestionCommand command)
    {
        var question = await questionRepository.FindById(command.QuestionId);

        if (question is null)
            throw new NotFoundException(nameof(Question), command.QuestionId);

        if (question.AuthorId != command.AuthorId)
            throw new ForbiddenException("You are not allowed to update this question.");

        question = QuestionEntity.Update(question, command.Title, command.Content, command.Slug);

        if (question.Attachments?.Count() > 0 && command.Attachments is not null)
        {
            await attachmentRepository.DeleteByQuestionId(question.Id);

            var attachments = new List<AttachmentEntity>();
            attachments.AddRange(command.Attachments.Select(att =>
                AttachmentEntity.Create(question.Id, AttachmentOwnerTypeEnum.Question, att.Title, att.Link)));

            await attachmentRepository.Create(attachments);
        }

        await questionRepository.Update(question);

        logger.LogInformation(LogType.Functional,
            $"The question with ID: {question.Id} was updated.");

        return Result<UpdateQuestionResult>.Success(new UpdateQuestionResult());
    }
}