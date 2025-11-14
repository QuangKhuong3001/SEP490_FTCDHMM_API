using System.ComponentModel.DataAnnotations;

namespace SEP490_FTCDHMM_API.Api.Dtos.UserDtos
{
    public class UpdateProfileRequest
    {
        [Required(ErrorMessage = "Vui lòng nhập họ.")]
        [StringLength(50, ErrorMessage = "Họ không được vượt quá 50 ký tự.")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập tên.")]
        [StringLength(50, ErrorMessage = "Tên không được vượt quá 50 ký tự.")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại.")]
        [RegularExpression(@"^0\d{8,9}$", ErrorMessage = "Số điện thoại phải bắt đầu bằng 0 và có từ 9 đến 10 chữ số.")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng chọn giới tính.")]
        public string Gender { get; set; } = string.Empty;

        public IFormFile? Avatar { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày sinh.")]
        public DateTime DateOfBirth { get; set; }

        [StringLength(256, ErrorMessage = "Địa chỉ không được vượt quá 256 ký tự.")]
        public string? Address { get; set; }

        [StringLength(256, ErrorMessage = "Giới thiệu bản thân không được vượt quá 256 ký tự.")]
        public string? Bio { get; set; }
    }
}
