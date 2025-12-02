namespace SEP490_FTCDHMM_API.Api.Dtos.NotificationDtos
{
    public class CreateNotificationRequest
    {
        public Guid ReceiverId { get; set; }
        public Guid? SenderId { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string? Link { get; set; }
    }
}
