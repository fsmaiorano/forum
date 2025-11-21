using Forum.Domain.Repositories;
using Forum.Infrastructure.Data.Context;

namespace Forum.Infrastructure.Data.Repositories;

public class AttachmentRepository(IForumDbContext context) : IAttachmentRepository
{
    public async Task Create(List<Attachment> attachments)
    {
        await context.Attachment.AddRangeAsync(attachments);
        await context.SaveChangesAsync();
    }
}