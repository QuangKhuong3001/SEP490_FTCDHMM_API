using SEP490_FTCDHMM_API.Application.Dtos.UserDtos;
using SEP490_FTCDHMM_API.Domain.ValueObjects;

namespace SEP490_FTCDHMM_API.Application.Dtos.NotificationDtos
{
    public class NotificationResponse
    {
        public Guid Id { get; set; }
        public NotificationType Type { get; set; } = NotificationType.System;
        public string? Message { get; set; }
        public Guid? TargetId { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public List<UserResponse> Senders { get; set; } = new List<UserResponse>();
    }
}
