using SEP490_FTCDHMM_API.Application.Dtos.DraftRecipeDtos.DraftCookingStep.DraftCookingStepImage;

namespace SEP490_FTCDHMM_API.Application.Dtos.DraftRecipeDtos.DraftCookingStep
{
    public class DraftCookingStepResponse
    {
        public Guid Id { get; set; }
        public string Instruction { get; set; } = string.Empty;
        public List<DraftCookingStepImageResponse> CookingStepImages { get; set; } = new();
        public int StepOrder { get; set; }
    }
}
