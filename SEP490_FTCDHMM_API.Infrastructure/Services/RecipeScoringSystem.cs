using Microsoft.Extensions.Options;
using SEP490_FTCDHMM_API.Application.Dtos.NutrientDtos.NutrientTarget;
using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos.Recommentdation;
using SEP490_FTCDHMM_API.Application.Interfaces.SystemServices;
using SEP490_FTCDHMM_API.Domain.Interfaces;
using SEP490_FTCDHMM_API.Domain.ValueObjects;
using SEP490_FTCDHMM_API.Infrastructure.ModelSettings;

namespace SEP490_FTCDHMM_API.Infrastructure.Services
{
    public class RecipeScoringSystem : IRecipeScoringSystem
    {
        private readonly FitScoreWeightsSettings _weights;
        private readonly MealDistributionSettings _mealDistribution;
        private readonly INutrientIdProvider _nutrientIdProvider;

        public RecipeScoringSystem(
            IOptions<FitScoreWeightsSettings> weights,
            IOptions<MealDistributionSettings> mealDistribution,
            INutrientIdProvider nutrientIdProvider)
        {
            _weights = weights.Value;
            _mealDistribution = mealDistribution.Value;
            _nutrientIdProvider = nutrientIdProvider;
        }

        public double CalculateFinalScore(
            RecommendationUserContext user,
            RecipeScoringSnapshot recipe)
        {
            if (IsRestricted(user, recipe))
                return 0;

            var nutrientFit = CalculateNutrientFit(recipe, user.Targets);
            var tdeeFit = CalculateTdeeFit(recipe, user.Tdee);
            var behaviorFit = CalculateBehaviorFit(recipe, user);

            var totalWeight =
                _weights.Nutrient +
                _weights.Tdee +
                _weights.Behavior;

            if (totalWeight == 0)
                return 0;

            return
                (_weights.Nutrient * nutrientFit +
                 _weights.Tdee * tdeeFit +
                 _weights.Behavior * behaviorFit)
                / totalWeight;
        }

        private bool IsRestricted(
            RecommendationUserContext user,
            RecipeScoringSnapshot recipe)
        {
            if (!user.RestrictedIngredientIds.Any() &&
                !user.RestrictedCategoryIds.Any())
                return false;

            if (recipe.IngredientIds.Any(user.RestrictedIngredientIds.Contains))
                return true;

            return recipe.IngredientCategoryIds.Any(user.RestrictedCategoryIds.Contains);
        }

        private double CalculateNutrientFit(
            RecipeScoringSnapshot recipe,
            List<NutrientTargetDto> targets)
        {
            if (!targets.Any())
                return 0;

            var nutrientMap = recipe.NutritionAggregates
                .GroupBy(n => n.NutrientId)
                .ToDictionary(g => g.Key, g => g.First().AmountPerServing);

            double total = 0;
            int count = 0;
            var perServingCalories = recipe.Calories / recipe.Ration;

            foreach (var t in targets)
            {
                if (!nutrientMap.TryGetValue(t.NutrientId, out var amount))
                    continue;

                double score = t.TargetType == NutrientTargetType.Absolute
                    ? ScoreAbsolute(amount, t)
                    : ScorePercentage(amount, t, perServingCalories);

                total += Math.Clamp(score, 0, 1);
                count++;
            }

            return count == 0 ? 0 : total / count;
        }

        private double ScoreAbsolute(decimal value, NutrientTargetDto t)
        {
            if (value < t.MinValue)
                return 1 - (double)((t.MinValue - value) / t.MinValue);

            if (value > t.MaxValue)
                return 1 - (double)((value - t.MaxValue) / t.MaxValue);

            return 1;
        }

        private double ScorePercentage(
            decimal amount,
            NutrientTargetDto t,
            decimal calories)
        {
            if (calories <= 0)
                return 0;

            var kcalPerGram = GetMacroCaloriesPerGram(t.NutrientId);
            if (kcalPerGram == 0)
                return 0;

            var pct = ((double)amount * kcalPerGram) / (double)calories * 100;

            var minPct = t.MinEnergyPct.HasValue
                ? (double)t.MinEnergyPct.Value
                : 0;

            var maxPct = t.MaxEnergyPct.HasValue
                ? (double)t.MaxEnergyPct.Value
                : 100;

            if (pct < minPct)
            {
                var diff = minPct - pct;
                return Math.Clamp(1 - diff / minPct, 0, 1);
            }

            if (pct > maxPct)
            {
                var diff = pct - maxPct;
                return Math.Clamp(1 - diff / maxPct, 0, 1);
            }

            return 1;
        }

        private double CalculateTdeeFit(RecipeScoringSnapshot recipe, double tdee)
        {
            if (tdee <= 0)
                return 0;

            var mealPct = GetMealDistribution();
            var targetCalories = tdee * mealPct;
            var perServing = (double)recipe.Calories / recipe.Ration;

            var diff = Math.Abs(perServing - targetCalories);
            return Math.Clamp(1 - diff / targetCalories, 0, 1);
        }

        private double CalculateBehaviorFit(RecipeScoringSnapshot recipe, RecommendationUserContext user)
        {
            if (recipe.LabelIds.Count == 0)
                return 0;

            double viewRatio = CalculateLabelRatio(
                recipe.LabelIds,
                user.ViewByLabel
            );

            double commentRatio = CalculateLabelRatio(
                recipe.LabelIds,
                user.CommentByLabel
            );

            double ratingRatio = CalculateLabelRatio(
                recipe.LabelIds,
                user.RatingByLabel
            );

            double saveRatio = CalculateLabelRatio(
                recipe.LabelIds,
                user.SaveByLabel
            );

            return
                0.3 * ratingRatio +
                0.4 * saveRatio +
                0.2 * commentRatio +
                0.1 * viewRatio;
        }

        private double CalculateLabelRatio(List<Guid> recipeLabels, Dictionary<Guid, int> userLabelStats)
        {
            if (userLabelStats.Count == 0)
                return 0;

            var total = userLabelStats.Values.Sum();
            if (total == 0)
                return 0;

            var matched = recipeLabels
                .Where(l => userLabelStats.ContainsKey(l))
                .Sum(l => userLabelStats[l]);

            return Math.Clamp((double)matched / total, 0, 1);
        }

        private double GetMealDistribution()
        {
            var now = DateTime.Now.TimeOfDay;

            if (now < new TimeSpan(11, 0, 0))
                return _mealDistribution.Breakfast;
            if (now < new TimeSpan(16, 0, 0))
                return _mealDistribution.Lunch;

            return _mealDistribution.Dinner;
        }

        private double GetMacroCaloriesPerGram(Guid nutrientId)
        {
            if (nutrientId == _nutrientIdProvider.ProteinId) return 4;
            if (nutrientId == _nutrientIdProvider.CarbohydrateId) return 4;
            if (nutrientId == _nutrientIdProvider.FatId) return 9;
            return 0;
        }
    }
}
