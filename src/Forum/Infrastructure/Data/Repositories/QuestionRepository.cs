using Forum.Domain.Repositories;
using Forum.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace Forum.Infrastructure.Data.Repositories;

public sealed class QuestionRepository(IForumDbContext context) : IQuestionRepository
{
    public async Task Create(Question question)
    {
        await context.Question.AddAsync(question);
        await context.SaveChangesAsync();
    }

    public async Task<Question?> FindById(string questionId, bool asNoTracking = false)
    {
        var query = context.Question.AsQueryable();
        if (asNoTracking) query = query.AsNoTracking();
        return await query.FirstOrDefaultAsync(q => q.Id.ToString().Equals(questionId));
    }

    public async Task Update(Question question)
    {
        question.Touch();
        context.Question.Update(question);
        await context.SaveChangesAsync();
    }
}