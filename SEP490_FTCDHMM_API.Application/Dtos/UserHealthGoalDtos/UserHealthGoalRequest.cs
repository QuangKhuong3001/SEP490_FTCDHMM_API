namespace SEP490_FTCDHMM_API.Application.Dtos.UserHealthGoalDtos
{
    public class UserHealthGoalRequest
    {
        public DateTime? ExpiredAtUtc { get; set; }
        public string Type { get; set; } = string.Empty;
    }
}
