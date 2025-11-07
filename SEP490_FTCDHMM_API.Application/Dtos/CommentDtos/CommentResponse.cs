namespace SEP490_FTCDHMM_API.Application.Dtos.CommentDtos
{
    public class CommentResponse
    {
        public Guid Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime CreatedAtUtc { get; set; }
        public Guid? ParentCommentId { get; set; }
        public Guid UserId { get; set; }

        public List<CommentResponse> Replies { get; set; } = new();
    }
}
