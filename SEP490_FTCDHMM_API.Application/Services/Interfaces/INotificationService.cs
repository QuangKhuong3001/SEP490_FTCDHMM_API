using SEP490_FTCDHMM_API.Application.Dtos.NotificationDtos;

namespace SEP490_FTCDHMM_API.Application.Services.Interfaces
{
    public interface INotificationService
    {
        Task<IEnumerable<NotificationResponse>> GetByUserIdAsync(Guid userId);
        Task CreateAsync(CreateNotificationRequest request);
        Task MarkAsReadAsync(Guid notificationId);
    }
}
