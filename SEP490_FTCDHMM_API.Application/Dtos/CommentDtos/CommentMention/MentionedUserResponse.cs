namespace SEP490_FTCDHMM_API.Application.Dtos.CommentDtos.CommentMention
{
    public class MentionedUserResponse
    {
        public Guid MentionedUserId { get; set; }
        public string UserName { get; set; } = string.Empty;
    }
}
