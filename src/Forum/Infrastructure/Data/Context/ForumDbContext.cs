using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace Forum.Infrastructure.Data.Context;

public interface IForumDbContext
{
    DbSet<Question> Question { get; }
    DbSet<Attachment> Attachment { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

public class ForumDbContext : DbContext, IForumDbContext
{
    public DbSet<Question> Question => Set<Question>();
    public DbSet<Attachment> Attachment => Set<Attachment>();

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