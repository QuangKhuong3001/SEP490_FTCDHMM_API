namespace SEP490_FTCDHMM_API.Application.Dtos.CommentDtos
{
    public class CreateCommentRequest
    {
        public string Content { get; set; } = string.Empty;
        public Guid? ParentCommentId { get; set; }
    }
}
