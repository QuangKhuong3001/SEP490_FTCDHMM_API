using System.ComponentModel.DataAnnotations;

namespace SEP490_FTCDHMM_API.Api.Dtos.RecipeDtos.CookingStep.CookingStepImage
{
    public class CookingStepImageRequest
    {
        /// New image file to upload (required if ExistingImageId is not provided)
        public IFormFile? Image { get; set; }

        /// ID of existing image to keep (used when publishing draft or updating recipe)
        public Guid? ExistingImageId { get; set; }

        [Required(ErrorMessage = "Thứ tự ảnh không được trống")]
        public int ImageOrder { get; set; }
    }
}
