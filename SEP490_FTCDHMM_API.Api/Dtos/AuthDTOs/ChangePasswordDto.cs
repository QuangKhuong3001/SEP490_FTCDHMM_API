using System.ComponentModel.DataAnnotations;

namespace SEP490_FTCDHMM_API.Api.Dtos.AuthDTOs
{
    public class ChangePasswordDto
    {
        [Required(ErrorMessage = "Missing Password")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be between 8 and 100 characters")]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Missing New Password")]
        public string NewPassword { get; set; } = string.Empty;
    }

}
