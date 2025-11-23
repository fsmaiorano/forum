using Forum.BuildingBlocks.Base;
using Forum.BuildingBlocks.Exceptions;
using Forum.BuildingBlocks.Logging;
using Forum.Domain;
using Forum.Domain.Enums;
using Forum.Domain.Repositories;

namespace Forum.Application.UseCases.Question.UpdateQuestion;

public record UpdateQuestionCommand(
    string QuestionId,
    string AuthorId,
    string Title,
    string Content,
    string? Slug = null,
    List<Attachment>? Attachments = null);

public record UpdateQuestionResult(bool IsSuccess);

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

        if (question.AuthorId.ToString() != command.AuthorId)
            throw new ForbiddenException("You are not allowed to update this question.");

        question = Domain.Entities.Question.Update(question, command.Title, command.Content, command.Slug);

        if (question.Attachments?.Count() > 0 && command.Attachments is not null)
        {
            await attachmentRepository.DeleteByQuestionId(question.Id);

            var attachments = new List<Attachment>();
            attachments.AddRange(command.Attachments.Select(att =>
                Attachment.Create(question.Id.ToString(), AttachmentOwnerType.Question, att.Title, att.Link)));

            await attachmentRepository.Create(attachments);
        }
        
        await questionRepository.Update(question);

        logger.LogInformation(LogType.Functional,
            $"The question with ID: {question.Id} was updated.");

        return Result<UpdateQuestionResult>.Success(new UpdateQuestionResult(true));
    }
}