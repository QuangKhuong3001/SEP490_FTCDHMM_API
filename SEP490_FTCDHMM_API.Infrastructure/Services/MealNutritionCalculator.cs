using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos.Recommentdation;
using SEP490_FTCDHMM_API.Application.Interfaces.SystemServices;
using SEP490_FTCDHMM_API.Domain.ValueObjects;

namespace SEP490_FTCDHMM_API.Infrastructure.Services
{
    public class MealNutritionCalculator : IMealNutritionCalculator
    {
        public MealNutritionState Calculate(IEnumerable<RecipeScoringSnapshot> recipes)
        {
            decimal calories = 0;
            var nutrients = new Dictionary<Guid, decimal>();

            foreach (var r in recipes)
            {
                var c = r.Ration <= 0 ? 0 : r.Calories / r.Ration;
                calories += c;

                foreach (var n in r.NutritionAggregates)
                {
                    if (!nutrients.ContainsKey(n.NutrientId))
                        nutrients[n.NutrientId] = 0;

                    nutrients[n.NutrientId] += n.AmountPerServing;
                }
            }

            return new MealNutritionState(calories, nutrients);
        }
    }
}
