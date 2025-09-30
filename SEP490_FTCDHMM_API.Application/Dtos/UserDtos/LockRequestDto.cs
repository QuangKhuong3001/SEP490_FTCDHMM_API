namespace SEP490_FTCDHMM_API.Application.Dtos.UserDtos
{
    public class LockRequestDto
    {
        public Guid UserId { get; set; }
        public int Day { get; set; } = 1;
    }
}
