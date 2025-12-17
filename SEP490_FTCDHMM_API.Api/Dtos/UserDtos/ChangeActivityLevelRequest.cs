using System.ComponentModel.DataAnnotations;

namespace SEP490_FTCDHMM_API.Api.Dtos.UserDtos
{
    public class ChangeActivityLevelRequest
    {
        [Required(ErrorMessage = "Vui lòng chọn mức hoạt động")]
        public string ActivityLevel { get; set; } = string.Empty;
    }
}
