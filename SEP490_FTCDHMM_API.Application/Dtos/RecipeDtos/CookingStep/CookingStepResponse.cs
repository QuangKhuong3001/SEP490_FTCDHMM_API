using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos.CookingStep.CookingStepImage;

namespace SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos.CookingStep
{
    public class CookingStepResponse
    {
        public Guid Id { get; set; }
        public required string Instruction { get; set; }
        public List<CookingStepImageResponse> CookingStepImages { get; set; } = new();
        public required int StepOrder { get; set; }
    }
}
