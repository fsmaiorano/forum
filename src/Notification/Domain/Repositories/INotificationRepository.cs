using BuildingBlocks.Base;
using Notification.Domain.Entities;

namespace Notification.Domain.Repositories;

public interface INotificationRepository
{
    public Task Create(NotificationEntity notificationEntity);
    public Task Update(NotificationEntity notificationEntity);
    public Task Delete(NotificationEntity notificationEntity);
    public Task<NotificationEntity?> FindById(UniqueEntityId notificationId, bool asNoTracking = false);
}