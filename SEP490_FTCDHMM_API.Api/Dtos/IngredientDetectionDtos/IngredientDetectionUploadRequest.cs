using System.ComponentModel.DataAnnotations;

namespace SEP490_FTCDHMM_API.Api.Dtos.IngredientDetectionDtos
{
    public class IngredientDetectionUploadRequest
    {
        [Required(ErrorMessage = "Missing image")]
        public required IFormFile Image { get; set; }
    }
}
