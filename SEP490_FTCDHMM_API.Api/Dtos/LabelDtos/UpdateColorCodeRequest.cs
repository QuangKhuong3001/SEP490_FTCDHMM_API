using System.ComponentModel.DataAnnotations;

namespace SEP490_FTCDHMM_API.Api.Dtos.LabelDtos
{
    public class UpdateColorCodeRequest
    {
        [Required(ErrorMessage = "Cần xác định thời gian cuối cùng chỉnh sửa mục tiêu.")]
        public DateTime? LastUpdatedUtc { get; set; }

        [Required(ErrorMessage = "Mã màu phải là hex format hợp lệ")]
        public string? ColorCode { get; set; }
    }
}
