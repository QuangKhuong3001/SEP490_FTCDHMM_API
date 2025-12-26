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
                .Select(r =>
                {
                    var baseScore = _recipeScoringSystem.CalculateFinalScore(user, r);

                    if (baseScore <= 0)
                        return (Recipe: r, Score: 0d);

                    double mealCompletionScore = 0;

                    var totalWeight = gap.RemainingNutrients.Sum(x => x.Weight);

                    foreach (var nt in r.NutritionAggregates)
                    {
                        var remain = gap.RemainingNutrients.FirstOrDefault(x => x.NutrientId == nt.NutrientId);
                        if (remain == null || remain.Min <= 0 || remain.Weight <= 0)
                            continue;

                        var ratio = Math.Min((double)(nt.AmountPerServing / remain.Min), 1.0);
                        var normalizedWeight = remain.Weight / totalWeight;

                        mealCompletionScore += ratio * normalizedWeight;
                    }

                    var finalScore = baseScore * (1 + 0.5 * mealCompletionScore);

                    return (Recipe: r, Score: finalScore);
                })
                .Where(x => x.Score > 0)
                .Where(x =>
                {
                    var addCalories = x.Recipe.Ration <= 0 ? 0 : x.Recipe.Calories / x.Recipe.Ration;
                    var afterCalories = current.Calories + addCalories;

                    if (afterCalories > maxCalories)
                        return false;

                    foreach (var nt in x.Recipe.NutritionAggregates)
                    {
                        var remain = gap.RemainingNutrients
                            .FirstOrDefault(x => x.NutrientId == nt.NutrientId);

                        if (remain == null)
                            continue;

                        if (remain.Max > 0 &&
                            nt.AmountPerServing > remain.Max &&
                            remain.Max != decimal.MaxValue)
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
