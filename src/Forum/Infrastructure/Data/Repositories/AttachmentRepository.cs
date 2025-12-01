using BuildingBlocks.Base;
using Forum.Domain.Enums;
using Forum.Domain.Repositories;
using Forum.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace Forum.Infrastructure.Data.Repositories;

public class AttachmentRepository(IForumDbContext context) : IAttachmentRepository
{
    public async Task Create(AttachmentEntity attachmentEntity)
    {
        await context.Attachment.AddAsync(attachmentEntity);
        await context.SaveChangesAsync();
    }
    
    public async Task Create(List<AttachmentEntity> attachments)
    {
        await context.Attachment.AddRangeAsync(attachments);
        await context.SaveChangesAsync();
    }

    public async Task<List<AttachmentEntity>> FindByOwnerId(UniqueEntityId questionId)
    {
        return await context.Attachment
            .Where(a => a.OwnerId == questionId)
            .ToListAsync();
    }

    public async Task Update(AttachmentEntity attachmentEntity)
    {
        context.Attachment.Update(attachmentEntity);
        await context.SaveChangesAsync();
    }

    public async Task Update(List<AttachmentEntity> attachments)
    {
        context.Attachment.UpdateRange(attachments);
        await context.SaveChangesAsync();
    }
    
    public async Task DeleteByOwnerId(UniqueEntityId ownerId)
    {
        var attachments = await context.Attachment
            .Where(a => a.OwnerId == ownerId)
            .ToListAsync();

        context.Attachment.RemoveRange(attachments);
        await context.SaveChangesAsync();
    }
}