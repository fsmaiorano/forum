using Forum.Domain;
using Forum.Domain.Entities.ValuesObjects;
using Forum.Domain.Enums;

namespace UnitTests.Factories;

public static class MakeAttachment
{
    public static Attachment Create(
        AttachmentOwnerType ownerType,
        string? title = null,
        string? link = null)
    {
        var faker = new Bogus.Faker();
        return Attachment.Create(
            new UniqueEntityId().ToString(),
            ownerType,
            title ?? faker.Lorem.Sentence(2),
            link ?? faker.Internet.Url());
    }
}