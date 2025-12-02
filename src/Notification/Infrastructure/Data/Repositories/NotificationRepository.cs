using BuildingBlocks.Base;
using Microsoft.EntityFrameworkCore;
using Notification.Domain.Entities;
using Notification.Domain.Repositories;
using Notification.Infrastructure.Data.Context;

namespace Notification.Infrastructure.Data.Repositories;

public sealed class NotificationRepository(INotificationDbContext context) : INotificationRepository
{
    public async Task Create(NotificationEntity notificationEntity)
    {
        await context.Notification.AddAsync(notificationEntity);
        await context.SaveChangesAsync();
    }

    public async Task Update(NotificationEntity notificationEntity)
    {
        notificationEntity.Touch();
        context.Notification.Update(notificationEntity);
        await context.SaveChangesAsync();
    }

    public async Task Delete(NotificationEntity notificationEntity)
    {
        context.Notification.Remove(notificationEntity);
        await context.SaveChangesAsync();
    }

    public async Task<NotificationEntity?> FindById(UniqueEntityId notificationId, bool asNoTracking = false)
    {
        var query = context.Notification.AsQueryable();
        if (asNoTracking) query = query.AsNoTracking();
        return await query.FirstOrDefaultAsync(n => n.Id.Equals(notificationId));
    }
}