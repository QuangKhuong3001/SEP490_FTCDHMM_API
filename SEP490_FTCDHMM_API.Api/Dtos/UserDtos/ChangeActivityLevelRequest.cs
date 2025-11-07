using System.ComponentModel.DataAnnotations;

namespace SEP490_FTCDHMM_API.Api.Dtos.UserDtos
{
    public class ChangeActivityLevelRequest
    {
        [Required(ErrorMessage = "Missing Activity Level")]
        public string ActivityLevel { get; set; } = string.Empty;
    }
}
