using System.ComponentModel.DataAnnotations;

namespace SEP490_FTCDHMM_API.Api.Dtos.IngredientDtos.IngredientDetection
{
    public class IngredientDetectionUploadRequest
    {
        [Required(ErrorMessage = "Thiếu ảnh")]
        public IFormFile? Image { get; set; }
    }
}
