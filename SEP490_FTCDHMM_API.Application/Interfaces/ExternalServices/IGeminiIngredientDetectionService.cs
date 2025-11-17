using Microsoft.AspNetCore.Http;
using SEP490_FTCDHMM_API.Application.Dtos.IngredientDtos.IngredientDetection;

namespace SEP490_FTCDHMM_API.Application.Interfaces.ExternalServices
{
    public interface IGeminiIngredientDetectionService
    {
        Task<List<IngredientDetectionResult>> DetectIngredientsAsync(IFormFile image);
    }
}
