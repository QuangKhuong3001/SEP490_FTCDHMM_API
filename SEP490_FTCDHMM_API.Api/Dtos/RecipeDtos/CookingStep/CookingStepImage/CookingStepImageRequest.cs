using System.ComponentModel.DataAnnotations;

namespace SEP490_FTCDHMM_API.Api.Dtos.RecipeDtos.CookingStep.CookingStepImage
{
    public class CookingStepImageRequest
    {
        // New image upload (required for new images, null when keeping existing)
        public IFormFile? Image { get; set; }

        // ID of an existing image to keep (used when updating recipe without changing the image)
        public Guid? ExistingImageId { get; set; }

        [Required(ErrorMessage = "Thứ tự ảnh không được trống")]
        public int ImageOrder { get; set; }
    }
}
