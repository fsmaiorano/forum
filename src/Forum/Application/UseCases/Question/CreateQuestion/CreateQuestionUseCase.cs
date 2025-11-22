using Forum.BuildingBlocks.Base;
using Forum.BuildingBlocks.Logging;
using Forum.Domain;
using Forum.Domain.Repositories;

namespace Forum.Application.UseCases.Question.CreateQuestion;

public record CreateQuestionCommand(
    string Title,
    string Content,
    string AuthorId,
    string? Slug = null,
    List<Attachment>? Attachments = null);

public record CreateQuestionResult(string QuestionId);

public interface ICreateQuestionUseCase
{
    Task<Result<CreateQuestionResult>> CreateQuestionUseCaseHandler(CreateQuestionCommand command);
}

public sealed class CreateQuestionUseCase(
    IAppLogger<CreateQuestionUseCase> logger,
    IQuestionRepository questionRepository,
    IAttachmentRepository attachmentRepository)
    : ICreateQuestionUseCase
{
    public async Task<Result<CreateQuestionResult>> CreateQuestionUseCaseHandler(CreateQuestionCommand command)
    {
        var question =
            Domain.Entities.Question.Create(
                command.AuthorId,
                command.Title,
                command.Content,
                command.Slug);

        if (command.Attachments?.Count > 0)
        {
            var attachments = new List<Attachment>();
            attachments.AddRange(command.Attachments.Select(att =>
                Attachment.Create(question.Id.ToString(), AttachmentOwnerType.Question, att.Title, att.Link)));
            
            await attachmentRepository.Create(attachments);
        }
        
        await questionRepository.Create(question);

        logger.LogInformation(LogType.Functional,
            $"The user {question.AuthorId} created a new question with ID: {question.Id}");

        return Result<CreateQuestionResult>.Success(new CreateQuestionResult(question.Id.ToString()));
    }
}