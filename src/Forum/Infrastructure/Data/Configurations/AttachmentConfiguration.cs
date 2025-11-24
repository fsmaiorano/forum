using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Forum.Infrastructure.Data.Configurations;

public class AttachmentConfiguration : IEntityTypeConfiguration<AttachmentEntity>
{
    public void Configure(EntityTypeBuilder<AttachmentEntity> builder)
    {
        builder.HasKey(m => m.Id);

        builder.Property(c => c.Id)
            .HasField("_id")
            .HasConversion(
                id => id.ToString(),
                value => UniqueEntityId.Of(value));

        builder.HasIndex(c => c.Id).IsUnique();

        builder.Property(c => c.OwnerId)
            .HasConversion(
                id => id.ToString(),
                value => UniqueEntityId.Of(value))
            .IsRequired();

        builder.Property(c => c.OwnerType)
            .IsRequired();

        builder.Property(c => c.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(c => c.Link)
            .IsRequired();
    }
}