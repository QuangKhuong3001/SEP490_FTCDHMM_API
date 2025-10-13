using System.ComponentModel.DataAnnotations;

namespace SEP490_FTCDHMM_API.Api.Dtos.AuthDTOs
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "Missing Email")]
        [EmailAddress(ErrorMessage = "Invalid Email Format")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Missing Password")]
        public string Password { get; set; } = string.Empty;
    }
}
