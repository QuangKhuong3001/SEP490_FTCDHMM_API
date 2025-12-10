using SEP490_FTCDHMM_API.Application.Dtos.NotificationDtos;

namespace SEP490_FTCDHMM_API.Application.Services.Interfaces
{
    public interface INotificationService
    {
        Task<IEnumerable<NotificationResponse>> GetNotificationsByUserIdAsync(Guid userId);
        Task MarkAsReadAsync(Guid userId, Guid notificationId);
    }
}
