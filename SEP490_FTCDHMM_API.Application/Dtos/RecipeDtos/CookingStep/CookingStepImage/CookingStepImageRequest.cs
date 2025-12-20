using SEP490_FTCDHMM_API.Application.Dtos.Common;

namespace SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos.CookingStep.CookingStepImage
{
    public class CookingStepImageRequest
    {
        public FileUploadModel Image { get; set; } = null!;
        public string? ExistingImageUrl { get; set; }
        public int ImageOrder { get; set; }
    }
}
