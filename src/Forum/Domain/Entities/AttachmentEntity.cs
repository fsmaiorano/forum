using Forum.BuildingBlocks.Base;
using Forum.Domain.Enums;

namespace Forum.Domain.Entities;

public sealed record AttachmentEntity : Entity
{
    public UniqueEntityId OwnerId { get; private set; } = null!;
    public AttachmentOwnerTypeEnum OwnerTypeEnum { get; private set; }
    public string Title { get; private set; } = null!;
    public string Link { get; private set; } = null!;


    public static AttachmentEntity Create(UniqueEntityId ownerId, AttachmentOwnerTypeEnum ownerTypeEnum, string title, string link)
    {
        var attachment = new AttachmentEntity()
        {
            Id = new UniqueEntityId(),
            OwnerId = ownerId,
            OwnerTypeEnum = ownerTypeEnum,
            Title = title,
            Link = link
        };

        return attachment;
    }

    public static AttachmentEntity Update(AttachmentEntity attachmentEntityToUpdate, string title, string link)
    {
        var attachment = attachmentEntityToUpdate with
        {
            Title = title,
            Link = link
        };

        return attachment;
    }
}