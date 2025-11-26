using System.ComponentModel.DataAnnotations;

namespace SEP490_FTCDHMM_API.Api.Dtos.UserHealthGoalDtos
{
    public class UserHealthGoalRequest
    {
        public DateTime? ExpiredAtUtc { get; set; }

        [Required(ErrorMessage = "Loại mục tiêu không được để trống")]
        public required string Type { get; set; }
    }
}
