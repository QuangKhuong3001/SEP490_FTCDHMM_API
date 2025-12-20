using SEP490_FTCDHMM_API.Application.Dtos.Common;

namespace SEP490_FTCDHMM_API.Application.Dtos.DraftRecipeDtos.DraftCookingStep.DraftCookingStepImage
{
    public class DraftCookingStepImageRequest
    {
        public FileUploadModel? Image { get; set; }
        public string? ExistingImageUrl { get; set; }
        public int ImageOrder { get; set; }
    }
}
