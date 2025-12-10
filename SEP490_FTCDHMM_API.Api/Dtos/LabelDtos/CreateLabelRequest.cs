using System.ComponentModel.DataAnnotations;

namespace SEP490_FTCDHMM_API.Api.Dtos.LabelDtos
{
    public class CreateLabelRequest
    {
        [Required(ErrorMessage = "Tên nhãn không được để trống")]
        [StringLength(255, MinimumLength = 1, ErrorMessage = "Tên nhãn phải từ 1 đến 255 ký tự")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mã màu không được để trống")]
        [RegularExpression(@"^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$", ErrorMessage = "Mã màu phải là hex format hợp lệ (ví dụ: #ffffff hoặc #fff)")]
        public string ColorCode { get; set; } = string.Empty;
    }
}
