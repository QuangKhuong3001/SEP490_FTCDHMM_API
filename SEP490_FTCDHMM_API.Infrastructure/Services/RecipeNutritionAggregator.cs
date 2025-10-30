﻿using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.Services;
using SEP490_FTCDHMM_API.Infrastructure.Data;

namespace SEP490_FTCDHMM_API.Infrastructure.Services
{
    public class RecipeNutritionAggregator : IRecipeNutritionAggregator
    {
        private readonly INutritionAnalyzer _analyzer;
        private readonly AppDbContext _appDbContext;

        public RecipeNutritionAggregator(INutritionAnalyzer analyzer, AppDbContext appDbContext)
        {
            _analyzer = analyzer;
            _appDbContext = appDbContext;
        }

        public async Task AggregateAndSaveAsync(Recipe recipe)
        {
            var profile = _analyzer.AnalyzeRecipe(recipe);

            var oldRecords = _appDbContext.RecipeNutritionAggregates.Where(x => x.RecipeId == recipe.Id);

            if (oldRecords.Any())
            {
                _appDbContext.RecipeNutritionAggregates.RemoveRange(oldRecords);
            }

            var aggregates = new List<RecipeNutritionAggregate>();

            foreach (var nutrient in recipe.RecipeIngredients
                                           .SelectMany(i => i.Ingredient.IngredientNutrients)
                                           .Select(n => n.Nutrient)
                                           .DistinctBy(n => n.Id))
            {
                profile.Nutrients.TryGetValue(nutrient.Name, out var amount);

                aggregates.Add(new RecipeNutritionAggregate
                {
                    RecipeId = recipe.Id,
                    NutrientId = nutrient.Id,
                    Amount = amount,
                    AmountPerServing = recipe.Ration > 0 ? amount / recipe.Ration : amount,
                    ComputedAtUtc = DateTime.UtcNow
                });
            }

            await _appDbContext.RecipeNutritionAggregates.AddRangeAsync(aggregates);
            await _appDbContext.SaveChangesAsync();
        }
    }
}
