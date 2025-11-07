using SEP490_FTCDHMM_API.Application.Dtos.IngredientDetectionDtos;
using SEP490_FTCDHMM_API.Application.Interfaces.ExternalServices;
using SEP490_FTCDHMM_API.Application.Interfaces.Persistence;
using SEP490_FTCDHMM_API.Application.Services.Interfaces;

namespace SEP490_FTCDHMM_API.Application.Services.Implementations
{
    public class IngredientDetectionService : IIngredientDetectionService
    {
        private readonly IGeminiIngredientDetectionService _geminiDetectionService;
        private readonly IIngredientRepository _ingredientRepo;

        public IngredientDetectionService(
            IGeminiIngredientDetectionService geminiDetectionService,
            IIngredientRepository ingredientRepo)
        {
            _geminiDetectionService = geminiDetectionService;
            _ingredientRepo = ingredientRepo;
        }

        public async Task<IEnumerable<IngredientDetectionResult>> DetectIngredientsAsync(IngredientDetectionUploadRequest request)
        {
            var result = await _geminiDetectionService.DetectIngredientsAsync(request.Image);

            if (!result.Any())
            {
                return new List<IngredientDetectionResult>();
            }

            var validIngredients = await _ingredientRepo.GetAllAsync();
            var validNames = validIngredients.Select(i => i.Name).ToHashSet(StringComparer.OrdinalIgnoreCase);

            return result.Where(r => validNames.Contains(r.Ingredient)).OrderByDescending(r => r.Confidence).ToList();
        }
    }
}
