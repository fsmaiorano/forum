using Forum.BuildingBlocks.Base;
using Forum.BuildingBlocks.Exceptions;
using Forum.BuildingBlocks.Logging;
using Forum.Domain.Repositories;

namespace Forum.Application.UseCases.Answer.UpdateAnswer;

public record UpdateAnswerCommand(
    UniqueEntityId AnswerId,
    UniqueEntityId AuthorId,
    string Title,
    string Content,
    string? Slug = null,
    List<AttachmentEntity>? Attachments = null);

public record UpdateAnswerResult();

public interface IUpdateAnswerUseCase
{
    Task<Result<UpdateAnswerResult>> UpdateAnswerUseCaseHandler(UpdateAnswerCommand command);
}

public class UpdateAnswerUseCase(
    IAppLogger<UpdateAnswerUseCase> logger,
    IAnswerRepository answerRepository,
    IAttachmentRepository attachmentRepository)
    : IUpdateAnswerUseCase
{
    public async Task<Result<UpdateAnswerResult>> UpdateAnswerUseCaseHandler(UpdateAnswerCommand command)
    {
        var answer = await answerRepository.FindById(command.AnswerId);

        if (answer is null)
            throw new NotFoundException(nameof(Answer), command.AnswerId);

        if (answer.AuthorId != command.AuthorId)
            throw new ForbiddenException("You are not allowed to update this answer.");

        answer = AnswerEntity.Update(answer, content: command.Content, attachments:
            command.Attachments is not null ? AttachmentList.Create(command.Attachments) : null);

        if (answer.Attachments?.GetItems().Count > 0 && command.Attachments is not null)
        {
            answer.Attachments.Update(command.Attachments);
            await attachmentRepository.Create(answer.Attachments.CurrentItems);
        }

        await answerRepository.Update(answer);

        logger.LogInformation(LogType.Functional,
            $"The answer with ID: {answer.Id} was updated.");

        return Result<UpdateAnswerResult>.Success(new UpdateAnswerResult());
    }
}