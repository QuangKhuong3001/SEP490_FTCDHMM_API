using System.ComponentModel.DataAnnotations;
using SEP490_FTCDHMM_API.Api.Dtos.CookingStepImageDtos;

namespace SEP490_FTCDHMM_API.Api.Dtos.CookingStepDtos
{
    public class CookingStepRequest
    {
        [Required(ErrorMessage = "Missing instruction")]
        public required string Instruction { get; set; }

        public List<CookingStepImageRequest> Images { get; set; } = new();

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "PageNumber must be greater than 0")]
        public required int StepOrder { get; set; }
    }
}
