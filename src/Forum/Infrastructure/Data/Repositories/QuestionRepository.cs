using Forum.Domain.Repositories;
using Forum.Infrastructure.Data.Context;

namespace Forum.Infrastructure.Data.Repositories;

public sealed class QuestionRepository(IForumDbContext context) : IQuestionRepository
{
    public async Task Create(Question question)
    {
        try
        {
            await context.Question.AddAsync(question);
            await context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
             throw new Exception("An error occurred while creating the question.", ex);
        }
    }
}