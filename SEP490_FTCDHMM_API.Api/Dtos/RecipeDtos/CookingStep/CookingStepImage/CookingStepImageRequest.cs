using System.ComponentModel.DataAnnotations;

namespace SEP490_FTCDHMM_API.Api.Dtos.RecipeDtos.CookingStep.CookingStepImage
{
    public class CookingStepImageRequest
    {
        public IFormFile? Image { get; set; }
        public string? ExistingImageUrl { get; set; }

        [Required(ErrorMessage = "Thứ tự ảnh không được trống")]
        public int ImageOrder { get; set; }
    }
}
