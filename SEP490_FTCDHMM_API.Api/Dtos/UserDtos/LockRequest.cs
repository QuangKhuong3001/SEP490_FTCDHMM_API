using System.ComponentModel.DataAnnotations;

namespace SEP490_FTCDHMM_API.Api.Dtos.UserDtos
{
    public class LockRequest
    {
        [Required(ErrorMessage = "Missing Time")]
        [Range(2, int.MaxValue, ErrorMessage = "Day must be greater than 1")]
        public int Day { get; set; } = 1;

        [StringLength(50, MinimumLength = 3, ErrorMessage = "Lí do không được để trống và không quá 512 ký tự.")]
        public required string Reason { get; set; }
    }
}
