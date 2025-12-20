namespace SEP490_FTCDHMM_API.Api.Dtos.DraftRecipeDtos.DraftCookingStep.DraftCookingStepImage
{
    public class DraftCookingStepImageRequest
    {
        public IFormFile? Image { get; set; }
        public string? ExistingImageUrl { get; set; }
        public int ImageOrder { get; set; }
    }
}
