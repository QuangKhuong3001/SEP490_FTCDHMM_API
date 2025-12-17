using System.ComponentModel.DataAnnotations;

namespace SEP490_FTCDHMM_API.Api.Dtos.UserDtos
{
    public class UnlockRequest
    {
        [Required(ErrorMessage = "Vui lòng chọn mục tiêu")]
        public Guid UserId { get; set; }
    }
}
