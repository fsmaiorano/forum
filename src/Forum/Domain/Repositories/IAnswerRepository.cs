using BuildingBlocks.Base;

namespace Forum.Domain.Repositories;

public interface IAnswerRepository
{
    public Task Create(AnswerEntity answerEntity);
    public Task Update(AnswerEntity answerEntity);
    public Task Delete(AnswerEntity answerEntity);
    public Task<AnswerEntity?> FindById(UniqueEntityId answerId, bool asNoTracking = false);
    public Task<IEnumerable<AnswerEntity>> GetByQuestionId(UniqueEntityId questionId);
}