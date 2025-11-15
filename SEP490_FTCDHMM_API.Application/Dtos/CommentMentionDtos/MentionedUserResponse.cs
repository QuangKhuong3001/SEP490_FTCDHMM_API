namespace SEP490_FTCDHMM_API.Application.Dtos.CommentMentionDtos
{
    public class MentionedUserResponse
    {
        public Guid MentionedUserId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
    }
}
