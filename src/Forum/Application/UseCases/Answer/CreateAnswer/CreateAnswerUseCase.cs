using BuildingBlocks.Base;
using BuildingBlocks.Logging;
using Forum.Domain.Enums;
using Forum.Domain.Repositories;
using Forum.Endpoints.Dtos;

namespace Forum.Application.UseCases.Answer.CreateAnswer;

public record CreateAnswerCommand(
    UniqueEntityId AuthorId,
    UniqueEntityId QuestionId,
    string Content,
    List<AttachmentEntity>? Attachments = null!);

public record CreateAnswerResult(string AnswerId);

public interface ICreateAnswerUseCase
{
    Task<Result<CreateAnswerResult>> CreateAnswerUseCaseHandler(CreateAnswerCommand command);
}

public sealed class CreateAnswerUseCase(
    IAppLogger<CreateAnswerUseCase> logger,
    IAnswerRepository answerRepository,
    IAttachmentRepository attachmentRepository) : ICreateAnswerUseCase
{
    public async Task<Result<CreateAnswerResult>> CreateAnswerUseCaseHandler(CreateAnswerCommand command)
    {
        var answer = AnswerEntity.Create(command.AuthorId, command.QuestionId, command.Content);

        if (command.Attachments is not null)
        {
            var attachments = new List<AttachmentEntity>();
            attachments.AddRange(command.Attachments.Select(att =>
                AttachmentEntity.Create(answer.Id, AttachmentOwnerTypeEnum.Answer, att.Title, att.Link)));

            await attachmentRepository.Create(attachments);
        }

        await answerRepository.Create(answer);

        logger.LogInformation(LogType.Functional,
            $"The user {answer.AuthorId} created a new answer with ID: {answer.Id} for question ID: {answer.QuestionId}");

        return Result<CreateAnswerResult>.Success(new CreateAnswerResult(answer.Id.ToString()));
    }
}