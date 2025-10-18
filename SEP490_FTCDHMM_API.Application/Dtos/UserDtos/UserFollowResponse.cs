namespace SEP490_FTCDHMM_API.Application.Dtos.UserDtos
{
    public class UserFollowResponse
    {
        public Guid UserFollowId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? AvatarUrl { get; set; }
    }
}
