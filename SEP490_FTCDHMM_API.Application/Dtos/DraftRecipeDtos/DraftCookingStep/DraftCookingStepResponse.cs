using SEP490_FTCDHMM_API.Application.Dtos.DraftRecipeDtos.DraftCookingStep.DraftCookingStepImage;

namespace SEP490_FTCDHMM_API.Application.Dtos.DraftRecipeDtos.DraftCookingStep
{
    public class DraftCookingStepResponse
    {
        public Guid Id { get; set; }
        public required string Instruction { get; set; }
        public List<DraftCookingStepImageResponse> CookingStepImages { get; set; } = new();
        public required int StepOrder { get; set; }
    }
}
