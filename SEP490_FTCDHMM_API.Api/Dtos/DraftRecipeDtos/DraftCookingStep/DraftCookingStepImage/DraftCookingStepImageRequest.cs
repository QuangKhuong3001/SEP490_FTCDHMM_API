namespace SEP490_FTCDHMM_API.Api.Dtos.DraftRecipeDtos.DraftCookingStep.DraftCookingStepImage
{
    public class DraftCookingStepImageRequest
    {
        public IFormFile Image { get; set; } = null!;
        public int ImageOrder { get; set; }
    }
}
