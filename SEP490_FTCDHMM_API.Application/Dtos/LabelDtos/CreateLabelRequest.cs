using System.ComponentModel.DataAnnotations;

namespace SEP490_FTCDHMM_API.Application.Dtos.LabelDtos
{
    public class CreateLabelRequest
    {
        [Required(ErrorMessage = "Tên nhãn không được để trống")]
        [StringLength(255, MinimumLength = 1, ErrorMessage = "Tên nhãn phải từ 1 đến 255 ký tự")]
        public required string Name { get; set; }

        [Required(ErrorMessage = "Mã màu không được để trống")]
        [RegularExpression(@"^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$", ErrorMessage = "Mã màu phải là hex format hợp lệ (ví dụ: #ffffff hoặc #fff)")]
        public required string ColorCode { get; set; }
    }
}
