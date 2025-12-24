using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos.Recommentdation;
using SEP490_FTCDHMM_API.Application.Interfaces.SystemServices;
using SEP490_FTCDHMM_API.Domain.ValueObjects;

namespace SEP490_FTCDHMM_API.Infrastructure.Services
{
    public class MealCompletionRecommender : IMealCompletionRecommender
    {
        private readonly IRecipeScoringSystem _recipeScoringSystem;

        public MealCompletionRecommender(IRecipeScoringSystem recipeScoringSystem)
        {
            _recipeScoringSystem = recipeScoringSystem;
        }

        public IEnumerable<(RecipeScoringSnapshot Recipe, double Score)> Recommend(
            RecommendationUserContext user,
            MealTarget target,
            MealNutritionState current,
            MealGap gap,
            IEnumerable<RecipeScoringSnapshot> candidates,
            ISet<Guid> excludedRecipeIds,
            int limit)
        {
            var maxCalories = target.TargetCalories * 1.1m;

            return candidates
                .Where(r => !excludedRecipeIds.Contains(r.Id))
                .Select(r => (Recipe: r, Score: _recipeScoringSystem.CalculateFinalScore(user, r)))
                .Where(x => x.Score > 0)
                .Where(x =>
                {
                    var addCalories = x.Recipe.Ration <= 0 ? 0 : x.Recipe.Calories / x.Recipe.Ration;
                    var afterCalories = current.Calories + addCalories;

                    if (afterCalories > maxCalories)
                        return false;

                    foreach (var nt in x.Recipe.NutritionAggregates)
                    {
                        if (!gap.RemainingNutrients.TryGetValue(nt.NutrientId, out var remain))
                            continue;

                        if (remain.Max > 0 && nt.AmountPerServing > remain.Max && remain.Max != decimal.MaxValue)
                            return false;
                    }

                    return true;
                })
                .OrderByDescending(x => x.Score)
                .Take(limit)
                .ToList();
        }
    }
}
