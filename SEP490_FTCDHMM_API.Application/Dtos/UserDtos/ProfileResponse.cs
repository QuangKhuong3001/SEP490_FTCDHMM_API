namespace SEP490_FTCDHMM_API.Application.Dtos.UserDtos
{
    public class ProfileResponse
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string? AvatarUrl { get; set; }
        public int FollowersCount { get; set; }
        public int FollowingCount { get; set; }
        public bool IsFollowing { get; set; }
        public string? Address { get; set; }
        public string? Bio { get; set; }
        public string UserName { get; set; } = string.Empty;
    }
}
