namespace SEP490_FTCDHMM_API.Application.Dtos.CommentDtos.CommentMention
{
    public class MentionedUserResponse
    {
        public Guid MentionedUserId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? AvatarUrl { get; set; }
    }
}
