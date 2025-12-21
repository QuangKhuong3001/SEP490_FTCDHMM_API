using SEP490_FTCDHMM_API.Domain.ValueObjects;

namespace SEP490_FTCDHMM_API.Application.Services.Interfaces
{
    public interface INotificationCommandService
    {
        Task CreateAndSendNotificationAsync(Guid? senderId, Guid receiverId, NotificationType type, Guid? targetId, string? message = null);
    }
}
