namespace SEP490_FTCDHMM_API.Application.Dtos.RatingDtos
{
    public class RatingRequest
    {
        public int Score { get; set; }
        public string? Feedback { get; set; } = string.Empty;
    }
}
