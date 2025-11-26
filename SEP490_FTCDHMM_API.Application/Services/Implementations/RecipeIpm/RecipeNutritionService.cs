using SEP490_FTCDHMM_API.Application.Services.Interfaces.RecipeInterface;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.Interfaces;

namespace SEP490_FTCDHMM_API.Application.Services.Implementations.RecipeIpm
{
    public class RecipeNutritionService : IRecipeNutritionService
    {
        private readonly IRecipeNutritionAggregator _recipeNutritionAggregator;

        public RecipeNutritionService(IRecipeNutritionAggregator recipeNutritionAggregator)
        {
            _recipeNutritionAggregator = recipeNutritionAggregator;
        }

        public async Task AggregateAsync(Recipe recipe)
        {
            await _recipeNutritionAggregator.AggregateAndSaveAsync(recipe);
        }

        public decimal GetCaloriesPerServing(Recipe recipe)
        {
            var ration = recipe.Ration <= 0 ? 1 : recipe.Ration;
            return recipe.Calories / ration;
        }
    }
}
