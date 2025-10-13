namespace SEP490_FTCDHMM_API.Application.Dtos.UserDtos
{
    public class LockResponse
    {
        public string Email { get; set; } = string.Empty;
        public DateTimeOffset? LockoutEnd { get; set; }
    }
}
