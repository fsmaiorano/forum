using Forum.Domain;
using Forum.Domain.Entities.ValuesObjects;
using Forum.Domain.Enums;

namespace UnitTests.Factories;

public static class MakeAttachment
{
    public static AttachmentEntity Create(
        UniqueEntityId ownerId,
        AttachmentOwnerTypeEnum ownerTypeEnum = AttachmentOwnerTypeEnum.Question,
        string? title = null,
        string? link = null)
    {
        var faker = new Bogus.Faker();
        return AttachmentEntity.Create(
            ownerId ?? new UniqueEntityId(),
            ownerTypeEnum,
            title ?? faker.Lorem.Sentence(2),
            link ?? faker.Internet.Url());
    }
}