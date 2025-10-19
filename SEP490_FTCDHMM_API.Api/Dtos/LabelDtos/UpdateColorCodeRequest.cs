using System.ComponentModel.DataAnnotations;

namespace SEP490_FTCDHMM_API.Api.Dtos.LabelDtos
{
    public class UpdateColorCodeRequest
    {
        [Required(ErrorMessage = "Missing color code")]
        public required string ColorCode { get; set; }
    }
}
