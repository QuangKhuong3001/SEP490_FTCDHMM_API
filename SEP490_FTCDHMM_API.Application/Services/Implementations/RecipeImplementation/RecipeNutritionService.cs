using SEP490_FTCDHMM_API.Application.Interfaces.Persistence;
using SEP490_FTCDHMM_API.Application.Services.Interfaces.RecipeInterfaces;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.Interfaces;

namespace SEP490_FTCDHMM_API.Application.Services.Implementations.RecipeImplementation
{
    public class RecipeNutritionService : IRecipeNutritionService
    {
        private readonly IRecipeNutritionAggregator _recipeNutritionAggregator;
        private readonly IRecipeRepository _recipeRepository;

        public RecipeNutritionService(IRecipeNutritionAggregator recipeNutritionAggregator, IRecipeRepository recipeRepository)
        {
            _recipeNutritionAggregator = recipeNutritionAggregator;
            _recipeRepository = recipeRepository;
        }

        public async Task AggregateRecipeAsync(Recipe recipe)
        {
            await _recipeNutritionAggregator.AggregateAndSaveAsync(recipe);
        }
    }
}
