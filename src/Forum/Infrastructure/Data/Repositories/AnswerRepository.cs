using BuildingBlocks.Base;
using Forum.Domain.Repositories;
using Forum.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace Forum.Infrastructure.Data.Repositories;

public sealed class AnswerRepository(IForumDbContext context) : IAnswerRepository
{
    public async Task Create(AnswerEntity answerEntity)
    {
        await context.Answer.AddAsync(answerEntity);
        await context.SaveChangesAsync();
    }

    public async Task Update(AnswerEntity answerEntity)
    {
        answerEntity.Touch();
        context.Answer.Update(answerEntity);
        await context.SaveChangesAsync();
    }

    public async Task Delete(AnswerEntity answerEntity)
    {
        context.Answer.Remove(answerEntity);
        await context.SaveChangesAsync();
    }

    public async Task<AnswerEntity?> FindById(UniqueEntityId answerId, bool asNoTracking = false)
    {
        var query = context.Answer.AsQueryable();
        if (asNoTracking) query = query.AsNoTracking();
        return await query.FirstOrDefaultAsync(q => q.Id.Equals(answerId));
    }
}