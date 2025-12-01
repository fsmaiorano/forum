using BuildingBlocks.Base;
using Notification.Domain.Entities;
using Notification.Domain.Repositories;

namespace Notification.Infrastructure.Data.Repositories;

public class NotificationRepository : INotificationRepository
{
    public Task Create(NotificationEntity notificationEntity)
    {
        throw new NotImplementedException();
    }

    public Task Update(NotificationEntity notificationEntity)
    {
        throw new NotImplementedException();
    }

    public Task Delete(NotificationEntity notificationEntity)
    {
        throw new NotImplementedException();
    }

    public Task<NotificationEntity?> FindById(UniqueEntityId notificationId, bool asNoTracking = false)
    {
        throw new NotImplementedException();
    }
}