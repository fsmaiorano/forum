namespace Forum.Domain.Repositories;

public interface IAttachmentRepository
{
    public Task Create(List<Attachment> attachments);
}