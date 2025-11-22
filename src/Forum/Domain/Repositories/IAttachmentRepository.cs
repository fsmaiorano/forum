namespace Forum.Domain.Repositories;

public interface IAttachmentRepository
{
    public Task Create(List<Attachment> attachments);
    public Task<List<Attachment>> FindByQuestionId(UniqueEntityId questionId);
    public Task DeleteByQuestionId(UniqueEntityId questionId);
}