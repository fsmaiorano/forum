using Forum.Domain.Repositories;
using Forum.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace Forum.Infrastructure.Data.Repositories;

public sealed class QuestionRepository(IForumDbContext context) : IQuestionRepository
{
    public async Task Create(QuestionEntity questionEntity)
    {
        await context.Question.AddAsync(questionEntity);
        await context.SaveChangesAsync();
    }

    public async Task<QuestionEntity?> FindById(UniqueEntityId questionId, bool asNoTracking = false)
    {
        var query = context.Question.AsQueryable();
        if (asNoTracking) query = query.AsNoTracking();
        return await query.FirstOrDefaultAsync(q => q.Id.Equals(questionId));
    }

    public async Task Update(QuestionEntity questionEntity)
    {
        questionEntity.Touch();
        context.Question.Update(questionEntity);
        await context.SaveChangesAsync();
    }

    public async Task Delete(QuestionEntity questionEntity)
    {
        context.Question.Remove(questionEntity);
        await context.SaveChangesAsync();
    }
}