using BuildingBlocks.Base;

namespace Forum.Domain.Repositories;

public interface IQuestionRepository
{
    public Task Create(QuestionEntity questionEntity);
    public Task Update(QuestionEntity questionEntity);
    public Task SelectBestAnswer(QuestionEntity questionEntity);
    public Task Delete(QuestionEntity questionEntity);
    public Task<QuestionEntity?> FindById(UniqueEntityId questionId, bool asNoTracking = false);
}