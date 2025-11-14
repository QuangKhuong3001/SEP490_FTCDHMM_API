namespace SEP490_FTCDHMM_API.Application.Dtos.UserDtos
{
    public class LockRequest
    {
        public int Day { get; set; } = 1;
        public string Reason { get; set; } = string.Empty;
    }
}
