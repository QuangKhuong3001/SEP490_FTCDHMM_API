using SEP490_FTCDHMM_API.Application.Dtos.IngredientDetectionDtos;

namespace SEP490_FTCDHMM_API.Application.Services.Interfaces
{
    public interface IIngredientDetectionService
    {
        Task<List<IngredientDetectionResult>> DetectIngredientsAsync(IngredientDetectionUploadRequest request);
    }
}
