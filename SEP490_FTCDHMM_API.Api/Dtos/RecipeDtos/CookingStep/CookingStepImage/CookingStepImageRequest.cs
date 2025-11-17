using System.ComponentModel.DataAnnotations;

namespace SEP490_FTCDHMM_API.Api.Dtos.RecipeDtos.CookingStep.CookingStepImage
{
    public class CookingStepImageRequest
    {
        [Required(ErrorMessage = "Ảnh không được trống")]
        public IFormFile Image { get; set; } = null!;

        [Required(ErrorMessage = "Thứ tự ảnh không được trống")]
        public int ImageOrder { get; set; }
    }
}
