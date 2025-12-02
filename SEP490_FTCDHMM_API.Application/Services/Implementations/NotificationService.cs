using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SEP490_FTCDHMM_API.Application.Dtos.NotificationDtos;
using SEP490_FTCDHMM_API.Application.Dtos.UserDtos;
using SEP490_FTCDHMM_API.Application.Interfaces.ExternalServices;
using SEP490_FTCDHMM_API.Application.Interfaces.Persistence;
using SEP490_FTCDHMM_API.Application.Services.Interfaces;
using SEP490_FTCDHMM_API.Domain.Entities;
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
                        response.Senders = new List<UserResponse>();
                    }
                    else
                    {
                        response.Senders = g
                            .Select(x => _mapper.Map<UserResponse>(x.Sender!))
                            .DistinctBy(u => u.Id)
                            .ToList();
                    }

                    return response;
                })
                .OrderByDescending(n => n.CreatedAtUtc)
                .ToList();

            return grouped;
        }



        public async Task CreateAsync(CreateNotificationRequest request)
        {
            var entity = _mapper.Map<Notification>(request);
            entity.CreatedAtUtc = DateTime.UtcNow;
            entity.IsRead = false;

            var saved = await _notificationRepository.AddAsync(entity);
            var result = _mapper.Map<NotificationResponse>(saved);


            await _notifier.SendNotificationAsync(request.ReceiverId, result);


        }

        public async Task MarkAsReadAsync(Guid notificationId)
        {
            var notification = await _notificationRepository.GetByIdAsync(notificationId);
            if (notification != null && !notification.IsRead)
            {
                notification.IsRead = true;
                await _notificationRepository.UpdateAsync(notification);
            }
        }
    }
}
