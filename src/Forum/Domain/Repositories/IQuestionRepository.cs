namespace Forum.Domain.Repositories;

public interface IQuestionRepository
{
    public Task Create(QuestionEntity questionEntity);
    public Task Update(QuestionEntity questionEntity);
    public Task<QuestionEntity?> FindById(string questionId, bool asNoTracking = false);
}