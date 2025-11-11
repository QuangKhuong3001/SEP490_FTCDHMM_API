using SEP490_FTCDHMM_API.Application.Dtos.UserDtos;

namespace SEP490_FTCDHMM_API.Application.Dtos.RatingDtos
{
    public class RatingResponse
    {
        public Guid Id { get; set; }
        public int Score { get; set; }
        public string Feedback { get; set; } = string.Empty;
        public UserInteractionResponse UserInteractionResponse { get; set; } = null!;
        public DateTime CreatedAtUtc { get; set; }
    }
}
