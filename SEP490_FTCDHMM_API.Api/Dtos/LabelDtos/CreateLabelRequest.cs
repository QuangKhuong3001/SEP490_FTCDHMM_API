using System.ComponentModel.DataAnnotations;

namespace SEP490_FTCDHMM_API.Api.Dtos.LabelDtos
{
    public class CreateLabelRequest
    {
        [Required(ErrorMessage = "Missing Name")]
        public required string Name { get; set; }

        [Required(ErrorMessage = "Missing Color Code")]
        public required string ColorCode { get; set; } = "#ffffff";

    }
}
