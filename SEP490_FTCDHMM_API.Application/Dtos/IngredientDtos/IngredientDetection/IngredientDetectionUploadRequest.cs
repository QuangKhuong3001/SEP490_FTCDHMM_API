using Microsoft.AspNetCore.Http;

namespace SEP490_FTCDHMM_API.Application.Dtos.IngredientDtos.IngredientDetection
{
    public class IngredientDetectionUploadRequest
    {
        public required IFormFile Image { get; set; }
    }
}
