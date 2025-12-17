using System.ComponentModel.DataAnnotations;

namespace SEP490_FTCDHMM_API.Api.Dtos.LabelDtos
{
    public class UpdateColorCodeRequest
    {
        [Required(ErrorMessage = "Cần xác định thời gian cuối cùng chỉnh sửa mục tiêu.")]
        public DateTime? LastUpdatedUtc { get; set; }

        [Required(ErrorMessage = "Missing color code")]
        public string ColorCode { get; set; } = string.Empty;
    }
}
