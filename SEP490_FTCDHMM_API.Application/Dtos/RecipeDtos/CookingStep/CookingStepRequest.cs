using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos.CookingStep.CookingStepImage;

namespace SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos.CookingStep
{
    public class CookingStepRequest
    {
        public string Instruction { get; set; } = string.Empty;
        public List<CookingStepImageRequest> Images { get; set; } = new();
        public int StepOrder { get; set; }
    }
}
