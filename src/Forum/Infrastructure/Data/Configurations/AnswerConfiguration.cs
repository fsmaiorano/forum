using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Forum.Infrastructure.Data.Configurations;

public class AnswerConfiguration : IEntityTypeConfiguration<AnswerEntity>
{
    public void Configure(EntityTypeBuilder<AnswerEntity> builder)
    {
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Id)
            .HasField("_id")
            .HasConversion(
                id => id.ToString(),
                value => UniqueEntityId.Of(value));

        builder.HasIndex(a => a.Id).IsUnique();
        
        builder.Property(a => a.QuestionId)
            .HasConversion(
                id => id.ToString(),
                value => UniqueEntityId.Of(value))
            .IsRequired();

        builder.Property(a => a.AuthorId)
            .HasConversion(
                id => id.ToString(),
                value => UniqueEntityId.Of(value))
            .IsRequired();
        
        builder.Property(a => a.Content)
            .IsRequired();
        
        builder.Property(a => a.IsClosed)
            .IsRequired();
        
        builder.Ignore(c => c.Attachments);
    }
}