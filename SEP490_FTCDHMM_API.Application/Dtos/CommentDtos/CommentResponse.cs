using SEP490_FTCDHMM_API.Application.Dtos.CommentDtos.CommentMention;
using SEP490_FTCDHMM_API.Application.Dtos.UserDtos;

namespace SEP490_FTCDHMM_API.Application.Dtos.CommentDtos
{
    public class CommentResponse
    {
        public Guid Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAtUtc { get; set; }
        public Guid? ParentCommentId { get; set; }
        public UserInteractionResponse User { get; set; } = new UserInteractionResponse();
        public List<CommentResponse> Replies { get; set; } = new();
        public List<MentionedUserResponse> Mentions { get; set; } = new();
    }
}
