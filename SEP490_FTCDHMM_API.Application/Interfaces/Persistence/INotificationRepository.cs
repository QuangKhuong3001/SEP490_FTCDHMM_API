using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.ValueObjects;

namespace SEP490_FTCDHMM_API.Application.Interfaces.Persistence
{
    public interface INotificationRepository : IRepository<Notification>
    {
        Task AddNotification(Guid? senderId, Guid receiverId, NotificationType type, string? message, Guid? targetId);
        Task AddNotifications(Guid? senderId, List<Guid> receiverIds, NotificationType type, string? message, Guid? targetId);
        Task AddNotificationToAll(Guid? senderId, NotificationType type, string? message, Guid? targetId);
    }
}

