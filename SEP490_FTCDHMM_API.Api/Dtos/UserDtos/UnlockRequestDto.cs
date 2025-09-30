using System.ComponentModel.DataAnnotations;

namespace SEP490_FTCDHMM_API.Api.Dtos.UserDtos
{
    public class UnlockRequestDto
    {
        [Required(ErrorMessage = "Missing UserId")]
        public Guid UserId { get; set; }
    }
}
