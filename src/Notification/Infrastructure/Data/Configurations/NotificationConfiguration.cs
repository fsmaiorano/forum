using BuildingBlocks.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Notification.Domain.Entities;

namespace Notification.Infrastructure.Data.Configurations;

public class NotificationConfiguration : IEntityTypeConfiguration<NotificationEntity>
{
    public void Configure(EntityTypeBuilder<NotificationEntity> builder)
    {
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Id)
            .HasField("_id")
            .HasConversion(
                id => id.ToString(),
                value => UniqueEntityId.Of(value));

        builder.HasIndex(a => a.Id).IsUnique();

        builder.Property(a => a.RecipientId)
            .HasConversion(
                id => id.ToString(),
                value => UniqueEntityId.Of(value))
            .IsRequired();

        builder.Property(a => a.Title)
            .IsRequired();

        builder.Property(a => a.Content)
            .IsRequired();

        builder.Property(a => a.ReadAt)
            .IsRequired(false);
        
        builder.Property(a => a.CreatedAt)
            .IsRequired();
        
        builder.Property(a => a.UpdatedAt)
            .IsRequired(false);
    }
}