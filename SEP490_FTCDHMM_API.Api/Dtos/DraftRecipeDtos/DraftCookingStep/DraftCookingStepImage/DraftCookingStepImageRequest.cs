namespace SEP490_FTCDHMM_API.Api.Dtos.DraftRecipeDtos.DraftCookingStep.DraftCookingStepImage
{
    public class DraftCookingStepImageRequest
    {
        /// New image file to upload (required if ExistingImageId is not provided)
        public IFormFile? Image { get; set; }

        /// ID of existing image to keep (used when updating draft without changing image)
        public Guid? ExistingImageId { get; set; }

        public int ImageOrder { get; set; }
    }
}
