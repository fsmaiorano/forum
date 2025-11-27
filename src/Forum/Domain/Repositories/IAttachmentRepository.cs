namespace Forum.Domain.Repositories;

public interface IAttachmentRepository
{
    public Task Create(AttachmentEntity attachmentEntity);
    public Task Create(List<AttachmentEntity> attachments);
    public Task<List<AttachmentEntity>> FindByOwnerId(UniqueEntityId questionId);
    public Task Update(AttachmentEntity attachmentEntity);
    public Task Update(List<AttachmentEntity> attachments);
    public Task DeleteByOwnerId(UniqueEntityId ownerId);
}