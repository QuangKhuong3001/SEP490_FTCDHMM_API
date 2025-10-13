using System.ComponentModel.DataAnnotations;

namespace SEP490_FTCDHMM_API.Api.Dtos.UserDtos
{
    public class LockRequest
    {
        [Required(ErrorMessage = "Missing UserId")]
        public Guid UserId { get; set; }

        [Required(ErrorMessage = "Missing Time")]
        [Range(2, int.MaxValue, ErrorMessage = "Day must be greater than 1")]
        public int Day { get; set; } = 1;
    }
}
