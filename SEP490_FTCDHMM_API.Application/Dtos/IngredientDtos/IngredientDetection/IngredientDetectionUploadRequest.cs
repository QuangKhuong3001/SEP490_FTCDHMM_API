using Microsoft.AspNetCore.Http;

namespace SEP490_FTCDHMM_API.Application.Dtos.IngredientDtos.IngredientDetection
{
    public class IngredientDetectionUploadRequest
    {
        public IFormFile? Image { get; set; }
    }
}
