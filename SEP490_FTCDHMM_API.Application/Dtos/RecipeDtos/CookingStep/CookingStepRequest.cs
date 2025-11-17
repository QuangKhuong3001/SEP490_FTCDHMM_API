using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos.CookingStep.CookingStepImage;

namespace SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos.CookingStep
{
    public class CookingStepRequest
    {
        public required string Instruction { get; set; }
        public List<CookingStepImageRequest> Images { get; set; } = new();
        public required int StepOrder { get; set; }
    }
}
