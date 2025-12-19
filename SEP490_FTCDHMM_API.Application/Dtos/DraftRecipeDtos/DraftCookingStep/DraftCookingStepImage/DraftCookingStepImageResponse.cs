namespace SEP490_FTCDHMM_API.Application.Dtos.DraftRecipeDtos.DraftCookingStep.DraftCookingStepImage
{
    public class DraftCookingStepImageResponse
    {
        public Guid Id { get; set; }
        public string? ImageUrl { get; set; }
        public int ImageOrder { get; set; }

        /// ID of the actual image (used to keep existing image when updating)
        public Guid ImageId { get; set; }
    }
}
