using Forum.BuildingBlocks.Base;
using Forum.Endpoints.Dtos;

namespace Forum.Application.UseCases.Answer.CreateAnswer;

public record CreateAnswerCommand(
    string Content,
    UniqueEntityId AuthorId,
    UniqueEntityId QuestionId,
    List<AttachmentEntity>? Attachments = null!);

public record CreateAnswerResult(string AnswerId);

public interface ICreateAnswerUseCase
{
    Task<Result<CreateAnswerResult>> CreateAnswerUseCaseHandler(CreateAnswerCommand command);
}

public class CreateAnswerUseCase : ICreateAnswerUseCase
{
    public Task<Result<CreateAnswerResult>> CreateAnswerUseCaseHandler(CreateAnswerCommand command)
    {
        // var answer = AnswerEntity.Create(
        
        return default;
    }
}