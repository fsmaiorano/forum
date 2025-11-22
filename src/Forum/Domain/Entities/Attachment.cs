using Forum.BuildingBlocks.Base;

namespace Forum.Domain.Entities;

public sealed record Attachment : Entity
{
    public UniqueEntityId OwnerId { get; private set; } = null!;
    public AttachmentOwnerType OwnerType { get; private set; }
    public string Title { get; private set; } = null!;
    public string Link { get; private set; } = null!;


    public static Attachment Create(string ownerId, AttachmentOwnerType ownerType, string title, string link)
    {
        var attachment = new Attachment()
        {
            Id = new UniqueEntityId(),
            OwnerId = new UniqueEntityId(ownerId),
            OwnerType = ownerType,
            Title = title,
            Link = link
        };

        return attachment;
    }

    public static Attachment Update(Attachment attachmentToUpdate, string title, string link)
    {
        var attachment = attachmentToUpdate with
        {
            Title = title,
            Link = link
        };

        return attachment;
    }
}