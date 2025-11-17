using System.ComponentModel.DataAnnotations;

namespace SEP490_FTCDHMM_API.Application.Dtos.CommentDtos
{
    public class CreateCommentRequest
    {
        [Required(ErrorMessage = "Bình luận không được để trống")]
        [StringLength(1000, MinimumLength = 1, ErrorMessage = "Bình luận phải từ 1 đến 1000 ký tự")]
        public string Content { get; set; } = string.Empty;

        public Guid? ParentCommentId { get; set; }
        public List<Guid> MentionedUserIds { get; set; } = new();

    }
}
