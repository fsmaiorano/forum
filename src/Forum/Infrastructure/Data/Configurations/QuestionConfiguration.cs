using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Forum.Infrastructure.Data.Configurations;

public class QuestionConfiguration : IEntityTypeConfiguration<Question>
{
    public void Configure(EntityTypeBuilder<Question> builder)
    {
        builder.HasKey(m => m.Id);
        
        // Configure Id property with backing field
        builder.Property(c => c.Id)
            .HasField("_id")
            .HasConversion(
                id => id.ToString(),
                value => UniqueEntityId.Of(value));
        
        builder.HasIndex(c => c.Id).IsUnique();
        
        // Configure AuthorId
        builder.Property(c => c.AuthorId)
            .HasConversion(
                id => id.ToString(),
                value => UniqueEntityId.Of(value))
            .IsRequired();
        
        // Configure BestAnswerId (optional)
        builder.Property(c => c.BestAnswerId)
            .HasConversion(
                id => id != null ? id.ToString() : null,
                value => value != null ? UniqueEntityId.Of(value) : null)
            .IsRequired(false);
        
        // Configure Slug (optional)
        builder.Property(c => c.Slug)
            .HasConversion(
                slug => slug != null ? slug.Value : null,
                value => value != null ? new Slug(value) : null)
            .IsRequired(false);
        
        // Configure other properties
        builder.Property(c => c.Title)
            .HasMaxLength(500)
            .IsRequired();
        
        builder.Property(c => c.Content)
            .IsRequired();
    }
}