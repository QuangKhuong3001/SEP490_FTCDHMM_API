namespace SEP490_FTCDHMM_API.Application.Dtos.UserDtos
{
    public class UserInteractionResponse
    {
        public Guid Id { get; set; }
        public string? AvatarUrl { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
