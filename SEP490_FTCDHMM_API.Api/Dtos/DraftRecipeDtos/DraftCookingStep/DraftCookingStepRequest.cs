using SEP490_FTCDHMM_API.Api.Dtos.DraftRecipeDtos.DraftCookingStep.DraftCookingStepImage;

namespace SEP490_FTCDHMM_API.Api.Dtos.DraftRecipeDtos.DraftCookingStep
{
    public class DraftCookingStepRequest
    {
        public string? Instruction { get; set; }
        public List<DraftCookingStepImageRequest> Images { get; set; } = new();
        public required int StepOrder { get; set; }
    }
}
