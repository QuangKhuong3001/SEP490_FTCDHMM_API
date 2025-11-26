namespace SEP490_FTCDHMM_API.Application.Dtos.UserHealthGoalDtos
{
    public class UserHealthGoalRequest
    {
        public DateTime? ExpiredAtUtc { get; set; }
        public required string Type { get; set; }
    }
}
