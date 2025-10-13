namespace SEP490_FTCDHMM_API.Application.Dtos.UserDtos
{
    public class LockRequest
    {
        public Guid UserId { get; set; }
        public int Day { get; set; } = 1;
    }
}
