using Microsoft.Extensions.Options;
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
            List<NutrientTarget> targets)
        {
            if (!targets.Any())
                return 0;

            var nutrientMap = recipe.NutritionAggregates
                .GroupBy(n => n.NutrientId)
                .ToDictionary(g => g.Key, g => g.First().AmountPerServing);

            double weightedTotal = 0;
            double weightSum = 0;
            var perServingCalories = recipe.Calories / recipe.Ration;

            foreach (var t in targets)
            {
                if (!nutrientMap.TryGetValue(t.NutrientId, out var amount))
                    continue;

                double rawScore = t.TargetType == NutrientTargetType.Absolute
                    ? ScoreAbsolute(amount, t)
                    : ScorePercentage(amount, t, perServingCalories);

                var score = 0.2 + 0.8 * Math.Clamp(rawScore, 0, 1);

                weightedTotal += score * t.Weight;
                weightSum += t.Weight;
            }

            return weightSum == 0 ? 0 : weightedTotal / weightSum;
        }


        private double ScoreAbsolute(decimal value, NutrientTarget t)
        {
            var min = t.MinValue <= 0 ? 0.0001m : t.MinValue;
            var max = t.MaxValue <= 0 ? decimal.MaxValue : t.MaxValue;

            if (value < min)
                return 1 - (double)((min - value) / min);

            if (value > max)
                return 1 - (double)((value - max) / max);

            return 1;
        }

        private double ScorePercentage(
            decimal amount,
            NutrientTarget t,
            decimal calories)
        {
            if (calories <= 0)
                return 0;

            var kcalPerGram = GetMacroCaloriesPerGram(t.NutrientId);
            if (kcalPerGram == 0)
                return 0;

            var pct = ((double)amount * kcalPerGram) / (double)calories * 100;

            var minPct = t.MinEnergyPct.HasValue ? (double)t.MinEnergyPct.Value : 0;
            var maxPct = t.MaxEnergyPct.HasValue ? (double)t.MaxEnergyPct.Value : 100;

            if (pct < minPct)
            {
                var diff = minPct - pct;
                return minPct == 0 ? 0 : Math.Clamp(1 - diff / minPct, 0, 1);
            }

            if (pct > maxPct)
            {
                var diff = pct - maxPct;
                return maxPct == 0 ? 0 : Math.Clamp(1 - diff / maxPct, 0, 1);
            }

            return 1;
        }

        private double CalculateTdeeFit(RecipeScoringSnapshot recipe, double tdee)
        {
            if (tdee <= 0)
                return 0;

            var mealPct = GetMealDistribution();
            var targetCalories = tdee * (double)mealPct;
            var perServing = (double)recipe.Calories / recipe.Ration;

            var diffRatio = Math.Abs(perServing - targetCalories) / targetCalories;

            return Math.Exp(-diffRatio * diffRatio * 4);
        }


        private double CalculateBehaviorFit(
    RecipeScoringSnapshot recipe,
    RecommendationUserContext user)
        {
            if (recipe.LabelIds.Count == 0)
                return 0;

            double viewRatio = CalculateLabelRatio(recipe.LabelIds, user.ViewByLabel);
            double commentRatio = CalculateLabelRatio(recipe.LabelIds, user.CommentByLabel);
            double ratingRatio = CalculateLabelRatio(recipe.LabelIds, user.RatingByLabel);
            double saveRatio = CalculateLabelRatio(recipe.LabelIds, user.SaveByLabel);

            var raw =
                0.4 * saveRatio +
                0.3 * ratingRatio +
                0.2 * commentRatio +
                0.1 * viewRatio;

            return Math.Log(1 + raw * 9) / Math.Log(10);
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

        private decimal GetMealDistribution()
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
