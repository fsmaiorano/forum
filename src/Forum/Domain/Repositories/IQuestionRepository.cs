namespace Forum.Domain.Repositories;

public interface IQuestionRepository
{
    public Task Create(Question question);
}