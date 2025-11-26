using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace Forum.Infrastructure.Data.Context;

public interface IForumDbContext
{
    DbSet<QuestionEntity> Question { get; }
    DbSet<AnswerEntity> Answer { get; }
    DbSet<AttachmentEntity> Attachment { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

public class ForumDbContext : DbContext, IForumDbContext
{
    public DbSet<QuestionEntity> Question => Set<QuestionEntity>();
    public DbSet<AnswerEntity> Answer => Set<AnswerEntity>();
    public DbSet<AttachmentEntity> Attachment => Set<AttachmentEntity>();

    public ForumDbContext(DbContextOptions<ForumDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }
}