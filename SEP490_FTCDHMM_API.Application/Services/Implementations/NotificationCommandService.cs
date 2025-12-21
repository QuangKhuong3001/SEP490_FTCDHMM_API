using SEP490_FTCDHMM_API.Application.Interfaces.Persistence;
using SEP490_FTCDHMM_API.Application.Interfaces.SystemServices;
using SEP490_FTCDHMM_API.Application.Services.Interfaces;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.ValueObjects;

namespace SEP490_FTCDHMM_API.Application.Services.Implementations
{
    public class NotificationCommandService : INotificationCommandService
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IRealtimeNotifier _notifier;
        public NotificationCommandService(
            INotificationRepository notificationRepository,
            IRealtimeNotifier notifier)
        {
            _notificationRepository = notificationRepository;
            _notifier = notifier;
        }

        public async Task CreateAndSendNotificationAsync(Guid? senderId, Guid receiverId, NotificationType type, Guid? targetId, string? message = null)
        {
            if (senderId == receiverId)
                return;

            var notification = new Notification
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                Type = type,
                TargetId = targetId,
                Message = message,
                CreatedAtUtc = DateTime.UtcNow,
            };

            await _notificationRepository.AddAsync(notification);

            await _notifier.SendNotificationAsync(receiverId, "notification_created");
        }
    }
}
