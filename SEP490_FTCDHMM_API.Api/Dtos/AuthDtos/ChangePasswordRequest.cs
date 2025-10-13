using System.ComponentModel.DataAnnotations;

namespace SEP490_FTCDHMM_API.Api.Dtos.AuthDTOs
{
    public class ChangePasswordRequest
    {
        [Required(ErrorMessage = "Missing Password")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be between 8 and 100 characters")]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Missing New Password")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be between 8 and 100 characters")]
        public string NewPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confirm password is required")]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match")]
        public string RePassword { get; set; } = string.Empty;
    }

}
