namespace SEP490_FTCDHMM_API.Application.Dtos.RatingDtos
{
    public class RatingResponse
    {
        public Guid Id { get; set; }
        public int Score { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime CreatedAtUtc { get; set; }
    }
}
