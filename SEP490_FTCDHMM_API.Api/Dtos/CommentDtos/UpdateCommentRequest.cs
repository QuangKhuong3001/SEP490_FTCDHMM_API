using System.ComponentModel.DataAnnotations;

namespace SEP490_FTCDHMM_API.Api.Dtos.CommentDtos
{
    public class UpdateCommentRequest
    {
        [StringLength(2048, MinimumLength = 1, ErrorMessage = "Nội dung không được để trống và không vượt quá 2048 ký tự")]
        public string Content { get; set; } = string.Empty;
        public List<Guid> MentionedUserIds { get; set; } = new();

    }
}
