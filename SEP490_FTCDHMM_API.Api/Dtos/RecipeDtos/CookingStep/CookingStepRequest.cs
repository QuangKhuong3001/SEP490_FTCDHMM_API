using System.ComponentModel.DataAnnotations;
using SEP490_FTCDHMM_API.Api.Dtos.RecipeDtos.CookingStep.CookingStepImage;

namespace SEP490_FTCDHMM_API.Api.Dtos.RecipeDtos.CookingStep
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
