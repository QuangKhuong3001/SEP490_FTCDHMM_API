using Microsoft.AspNetCore.Http;

namespace SEP490_FTCDHMM_API.Application.Dtos.IngredientDetectionDtos
{
    public class IngredientDetectionUploadRequest
    {
        public required IFormFile Image { get; set; }
    }
}
