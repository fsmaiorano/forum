using Forum.Domain.Enums;
using UnitTests.Factories;

namespace UnitTests.Repositories;

public class AttachmentRepositoryUnitTest(TestFixture fixture) : BaseTest(fixture)
{
    [Fact]
    public async Task Create_ShouldAddAttachmentToAnQuestion()
    {
        var repository = new AttachmentRepository(Context);
        var question = MakeQuestion.Create();
        var attachment = MakeAttachment.Create(question.AuthorId, AttachmentOwnerTypeEnum.Question);
        await repository.Create(attachment);
    }

    [Fact]
    public async Task Create_ShouldAddMultipleAttachmentsToAnQuestion()
    {
        var repository = new AttachmentRepository(Context);
        var question = MakeQuestion.Create();
        var attachments = new List<AttachmentEntity>
        {
            MakeAttachment.Create(question.AuthorId,AttachmentOwnerTypeEnum.Question),
            MakeAttachment.Create(question.AuthorId,AttachmentOwnerTypeEnum.Question),
        };

        await repository.Create(attachments);
    }

    [Fact]
    public async Task FindByQuestionId_ShouldReturnAttachmentsByQuestionId()
    {
        var repository = new AttachmentRepository(Context);
        var question = MakeQuestion.Create();
        var attachments = new List<AttachmentEntity>
        {
            MakeAttachment.Create(question.Id ,AttachmentOwnerTypeEnum.Question),
            MakeAttachment.Create(question.Id ,AttachmentOwnerTypeEnum.Question),
        };

        await repository.Create(attachments);
        var storedAttachments = await repository.FindByQuestionId(question.Id); 

        Assert.Equal(attachments.Count, storedAttachments.Count);
    }

    [Fact]
    public async Task DeleteByQuestionId_ShouldDeleteAttachmentsByQuestionId()
    {
        var repository = new AttachmentRepository(Context);
        var question = MakeQuestion.Create();
        var attachments = new List<AttachmentEntity>
        {
            MakeAttachment.Create(question.Id,AttachmentOwnerTypeEnum.Question),
            MakeAttachment.Create(question.Id,AttachmentOwnerTypeEnum.Question),
        };

        await repository.Create(attachments);
        await repository.DeleteByOwnerId(question.Id);

        var storedAttachments = Context.Attachment.Where(a => a.OwnerId.Equals(question.Id)).ToList();
        Assert.Empty(storedAttachments);
    }
}