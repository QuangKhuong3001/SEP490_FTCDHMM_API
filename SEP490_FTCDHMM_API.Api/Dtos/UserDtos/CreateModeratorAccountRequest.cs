using System.ComponentModel.DataAnnotations;

namespace SEP490_FTCDHMM_API.Api.Dtos.UserDtos
{
    public class CreateModeratorAccountRequest
    {
        [Required(ErrorMessage = "Missing Email")]
        [EmailAddress(ErrorMessage = "Invalid Email Format")]
        public string Email { get; set; } = string.Empty;
    }
}
