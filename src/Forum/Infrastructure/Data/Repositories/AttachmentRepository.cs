using Forum.Domain.Enums;
using Forum.Domain.Repositories;
using Forum.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace Forum.Infrastructure.Data.Repositories;

public class AttachmentRepository(IForumDbContext context) : IAttachmentRepository
{
    public async Task Create(Attachment attachment)
    {
        await context.Attachment.AddAsync(attachment);
        await context.SaveChangesAsync();
    }
    
    public async Task Create(List<Attachment> attachments)
    {
        await context.Attachment.AddRangeAsync(attachments);
        await context.SaveChangesAsync();
    }

    public async Task<List<Attachment>> FindByQuestionId(UniqueEntityId questionId)
    {
        return await context.Attachment
            .Where(a => a.OwnerId == questionId)
            .ToListAsync();
    }
    
    public async Task DeleteByQuestionId(UniqueEntityId questionId)
    {
        var attachments = await context.Attachment
            .Where(a => a.OwnerId == questionId)
            .ToListAsync();

        context.Attachment.RemoveRange(attachments);
        await context.SaveChangesAsync();
    }
}