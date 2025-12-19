using SEP490_FTCDHMM_API.Application.Dtos.Common;
using SEP490_FTCDHMM_API.Application.Dtos.NotificationDtos;

namespace SEP490_FTCDHMM_API.Application.Services.Interfaces
{
    public interface INotificationService
    {
        Task<PagedResult<NotificationResponse>> GetNotificationsByUserIdAsync(
            Guid userId,
            PaginationParams pagination);
        Task MarkAsReadAsync(Guid userId, Guid notificationId);
    }
}
