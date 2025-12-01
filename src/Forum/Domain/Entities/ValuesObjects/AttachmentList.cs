using BuildingBlocks.Base;

namespace Forum.Domain.Entities.ValuesObjects;

public class AttachmentList : WatchedList<AttachmentEntity>
{
    private AttachmentList(List<AttachmentEntity>? initialItems = null) : base(initialItems)
    {
    }

    public static AttachmentList Create(List<AttachmentEntity>? initialItems = null) => new(initialItems);

    protected override bool CompareItems(AttachmentEntity a, AttachmentEntity b) => a.Id == b.Id;
}