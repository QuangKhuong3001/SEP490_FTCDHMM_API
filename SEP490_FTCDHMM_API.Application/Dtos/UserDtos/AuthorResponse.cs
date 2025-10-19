namespace SEP490_FTCDHMM_API.Application.Dtos.UserDtos
{
    public class AuthorResponse
    {
        public Guid Id { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public string? AvatarUrl { get; set; }
        public DateTime CreatedAtUtc { get; set; }
    }
}
