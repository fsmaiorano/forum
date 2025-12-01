using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Notification.Domain.Entities;

namespace Notification.Infrastructure.Data.Context;

public interface INotificationDbContext
{
    DbSet<NotificationEntity> Notification { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

public class NotificationDbContext : DbContext, INotificationDbContext
{
    public DbSet<NotificationEntity> Notification => Set<NotificationEntity>();

    public NotificationDbContext(DbContextOptions<NotificationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }
}