using System.ComponentModel.DataAnnotations;

namespace SEP490_FTCDHMM_API.Api.Dtos.UserDtos
{
    public class UpdateProfileRequest
    {
        [Required(ErrorMessage = "Vui lòng nhập tên")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Tên phải từ 1-50 ký tự")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập họ")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Họ phải từ 1-50 ký tự")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng chọn giới tính")]
        public string Gender { get; set; } = string.Empty;
        public IFormFile? Avatar { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày sinh")]
        public DateTime DateOfBirth { get; set; }
    }
}
