using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SEP490_FTCDHMM_API.Application.Dtos.NotificationDtos;
using SEP490_FTCDHMM_API.Application.Dtos.UserDtos;
using SEP490_FTCDHMM_API.Application.Interfaces.Persistence;
using SEP490_FTCDHMM_API.Application.Interfaces.SystemServices;
using SEP490_FTCDHMM_API.Application.Services.Interfaces;
using SEP490_FTCDHMM_API.Domain.ValueObjects;

namespace SEP490_FTCDHMM_API.Application.Services.Implementations
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IMapper _mapper;
        private readonly IRealtimeNotifier _notifier;
        public NotificationService(INotificationRepository notificationRepository, IMapper mapper, IRealtimeNotifier notifier)
        {
            _notificationRepository = notificationRepository;
            _mapper = mapper;
            _notifier = notifier;
        }

        public async Task<IEnumerable<NotificationResponse>> GetByUserIdAsync(Guid userId)
        {
            var notifications = await _notificationRepository.GetAllAsync(
                n => n.ReceiverId == userId,
                q => q
                    .Include(x => x.Sender)
                    .OrderByDescending(x => x.CreatedAtUtc)
            );

            var grouped = notifications
                .GroupBy(n => new
                {
                    n.TargetId,
                    n.Type,
                    Date = n.CreatedAtUtc.Date
                })
                .Select(g =>
                {
                    var response = _mapper.Map<NotificationResponse>(g.First());

                    if (response.Type == NotificationType.System)
                    {
                        response.Senders = new List<UserInteractionResponse>();
                    }

                    return response;
                })
                .OrderByDescending(n => n.CreatedAtUtc)
                .ToList();

            return grouped;
        }

        public async Task MarkAsReadAsync(Guid userId, Guid notificationId)
        {
            var notification = await _notificationRepository.GetByIdAsync(notificationId);

            if (notification != null && !notification.IsRead)
            {
                notification.IsRead = true;
                await _notificationRepository.UpdateAsync(notification);
                await _notifier.SendMarkNotificationAsReadAsync(userId, notificationId);
            }
        }
    }
}
