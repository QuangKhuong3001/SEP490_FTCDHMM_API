using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos.CookingStep.CookingStepImage;

namespace SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos.CookingStep
{
    public class CookingStepResponse
    {
        public Guid Id { get; set; }
        public string Instruction { get; set; } = string.Empty;
        public List<CookingStepImageResponse> CookingStepImages { get; set; } = new();
        public int StepOrder { get; set; }
    }
}
