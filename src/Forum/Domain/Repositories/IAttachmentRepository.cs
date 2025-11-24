namespace Forum.Domain.Repositories;

public interface IAttachmentRepository
{
    public Task Create(Attachment attachment);
    public Task Create(List<Attachment> attachments);
    public Task<List<Attachment>> FindByQuestionId(UniqueEntityId questionId);
    public Task Update(Attachment attachment);
    public Task Update(List<Attachment> attachments);
    public Task DeleteByQuestionId(UniqueEntityId questionId);
}