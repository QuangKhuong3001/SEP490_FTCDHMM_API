using Microsoft.EntityFrameworkCore;
using SEP490_FTCDHMM_API.Application.Interfaces.Persistence;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.ValueObjects;
using SEP490_FTCDHMM_API.Infrastructure.Data;

namespace SEP490_FTCDHMM_API.Infrastructure.Repositories
{
    public class NotificationRepository : EfRepository<Notification>, INotificationRepository
    {
        private readonly AppDbContext _dbContext;

        public NotificationRepository(AppDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddNotification(Guid? senderId, Guid receiverId, NotificationType type, string? message, Guid? targetId)
        {
            // Don't create notification if sender and receiver are the same
            if (senderId.HasValue && senderId.Value == receiverId)
                return;

            var notification = new Notification
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                Type = type,
                Message = message,
                TargetId = targetId,
                CreatedAtUtc = DateTime.UtcNow,
            };

            await _dbContext.Notifications.AddAsync(notification);
            await _dbContext.SaveChangesAsync();
        }

        public async Task AddNotifications(Guid? senderId, List<Guid> receiverIds, NotificationType type, string? message, Guid? targetId)
        {
            var notifications = new List<Notification>();

            foreach (var receiverId in receiverIds)
            {
                // Skip if sender and receiver are the same
                if (senderId.HasValue && senderId.Value == receiverId)
                    continue;

                var notification = new Notification
                {
                    SenderId = senderId,
                    ReceiverId = receiverId,
                    Type = type,
                    Message = message,
                    TargetId = targetId,
                    CreatedAtUtc = DateTime.UtcNow,
                };
                notifications.Add(notification);
            }

            if (notifications.Any())
            {
                await _dbContext.Notifications.AddRangeAsync(notifications);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task AddNotificationToAll(Guid? senderId, NotificationType type, string? message, Guid? targetId)
        {
            var notifications = new List<Notification>();
            var userIds = await _dbContext.Users.Where(u => u.EmailConfirmed == true).Select(u => u.Id).ToListAsync();
            foreach (var receiverId in userIds)
            {
                var notification = new Notification
                {
                    SenderId = senderId,
                    ReceiverId = receiverId,
                    Type = type,
                    Message = message,
                    TargetId = targetId,
                    CreatedAtUtc = DateTime.UtcNow,
                };
                notifications.Add(notification);
            }

            await _dbContext.Notifications.AddRangeAsync(notifications);
            await _dbContext.SaveChangesAsync();
        }
    }
}
