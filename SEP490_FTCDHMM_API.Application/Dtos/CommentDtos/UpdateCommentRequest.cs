namespace SEP490_FTCDHMM_API.Application.Dtos.CommentDtos
{
    public class UpdateCommentRequest
    {
        public string Content { get; set; } = string.Empty;
        public List<Guid> MentionedUserIds { get; set; } = new();

    }
}
