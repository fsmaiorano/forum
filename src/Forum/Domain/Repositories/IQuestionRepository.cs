namespace Forum.Domain.Repositories;

public interface IQuestionRepository
{
    public Task Create(Question question);
    public Task Update(Question question);
    public Task<Question?> FindById(string questionId, bool asNoTracking = false);
}