using SEP490_FTCDHMM_API.Domain.ValueObjects;

namespace SEP490_FTCDHMM_API.Domain.Entities
{
    public class Notification
    {
        public Guid Id { get; set; }
        public Guid ReceiverId { get; set; }
        public AppUser Receiver { get; set; } = null!;
        public Guid? SenderId { get; set; }
        public AppUser? Sender { get; set; }
        public NotificationType Type { get; set; } = NotificationType.System;
        public string? Message { get; set; }
        public Guid? TargetId { get; set; }
        public bool IsRead { get; set; } = false;
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    }
}
