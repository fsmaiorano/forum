using Forum.Domain.Repositories;
using Forum.Infrastructure.Data.Context;

namespace Forum.Infrastructure.Data.Repositories;

public class QuestionRepository(IForumDbContext context) : IQuestionRepository
{
    public async Task Create(Question question)
    {
        await context.Question.AddAsync(question);
        await context.SaveChangesAsync();
    }
}