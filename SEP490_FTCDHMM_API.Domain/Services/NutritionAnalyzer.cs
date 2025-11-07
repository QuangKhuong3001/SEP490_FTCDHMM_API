using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.ValueObjects;

namespace SEP490_FTCDHMM_API.Domain.Services
{
    public static class NutritionAnalyzer
    {
        public static NutritionProfile AnalyzeRecipe(Recipe recipe)
        {
            var totals = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase);
            decimal totalCalories = 0;

            foreach (var ri in recipe.RecipeIngredients)
            {
                var qtyFactor = ri.QuantityGram <= 0 ? 0 : ri.QuantityGram / 100m;

                foreach (var inut in ri.Ingredient.IngredientNutrients)
                {
                    var nutrientName = inut.Nutrient.Name;
                    var addValue = inut.MedianValue * qtyFactor;

                    if (addValue == 0) continue;

                    if (totals.TryGetValue(nutrientName, out var current))
                        totals[nutrientName] = current + addValue;
                    else
                        totals[nutrientName] = addValue;
                }

                totalCalories += ri.Ingredient.Calories * (ri.QuantityGram / 100m);
            }

            return new NutritionProfile
            {
                TotalCalories = Math.Round(totalCalories, 2),
                Nutrients = totals
            };
        }
    }
}
