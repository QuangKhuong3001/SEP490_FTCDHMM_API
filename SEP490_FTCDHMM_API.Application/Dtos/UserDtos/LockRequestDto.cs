namespace SEP490_FTCDHMM_API.Application.Dtos.UserDtos
{
    public class LockRequestDto
    {
        public string UserId { get; set; } = string.Empty;
        public int Day { get; set; } = 1;
    }
}
