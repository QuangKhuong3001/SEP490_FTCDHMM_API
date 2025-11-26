using Microsoft.Extensions.Options;
using SEP490_FTCDHMM_API.Application.Interfaces.SystemServices;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.Interfaces;
using SEP490_FTCDHMM_API.Domain.ValueObjects;
using SEP490_FTCDHMM_API.Infrastructure.ModelSettings;

namespace SEP490_FTCDHMM_API.Infrastructure.Services
{
    public class RecipeScoringSystem : IRecipeScoringSystem
    {
        private readonly FitScoreWeightsSettings _weightsSettings;
        private readonly INutrientIdProvider _nutrientIdProvider;

        public RecipeScoringSystem(IOptions<FitScoreWeightsSettings> weightsSettings, INutrientIdProvider nutrientIdProvider)
        {
            _weightsSettings = weightsSettings.Value;
            _nutrientIdProvider = nutrientIdProvider;
        }

        public double CalculateFinalScore(AppUser user, Recipe recipe)
        {
            var nutrientFit = CalculateNutrientFit(recipe, user.UserHealthGoal);
            var tdeeFit = CalculateTdeeFit(recipe, user.HealthMetrics.FirstOrDefault());
            //var behaviorFit = this.CalculateBehaviorFit(user);
            //var popularity = CalculatePopularityScore(recipe);
            //var freshness = this.CalculateFreshnessScore(recipe.CreatedAtUtc);

            var finalScore =
                (_weightsSettings.Nutrient * nutrientFit + _weightsSettings.Tdee * tdeeFit
                //+0.15 * popularity+ 0.22 * behaviorFit + 0.15 * freshness
                )
                /
                (_weightsSettings.Nutrient + _weightsSettings.Tdee
                //+ 0.15+ 0.22 +0.15 
                );

            return finalScore;
        }
        private double CalculateNutrientFit(Recipe recipe, UserHealthGoal? userGoal)
        {
            if (recipe == null || userGoal == null)
                return 0;

            IEnumerable<HealthGoalTarget>? targets = null;

            if (userGoal.Type == HealthGoalType.SYSTEM)
                targets = userGoal.HealthGoal?.Targets;

            else if (userGoal.Type == HealthGoalType.CUSTOM)
                targets = userGoal.CustomHealthGoal?.Targets;

            if (targets == null || !targets.Any())
                return 0;

            var nutrientDict = recipe.NutritionAggregates
                .GroupBy(n => n.NutrientId)
                .ToDictionary(g => g.Key, g => g.First().AmountPerServing);

            double totalScore = 0;
            int count = 0;

            foreach (var target in targets)
            {
                if (!nutrientDict.TryGetValue(target.NutrientId, out var recipeAmount))
                    continue;

                double score;

                if (target.TargetType == NutrientTargetType.Absolute)
                {
                    score = CalculateAbsoluteNutrientScore(recipeAmount, target);
                }
                else // Percentage
                {
                    score = CalculatePercentageNutrientScore(recipeAmount, target, recipe.Calories);
                }

                score = Math.Clamp(score, 0, 1);

                totalScore += score;
                count++;
            }

            return count == 0 ? 0 : totalScore / count;
        }

        private double CalculateAbsoluteNutrientScore(decimal recipeAmount, HealthGoalTarget target)
        {
            var score = 0.0;
            if (recipeAmount < target.MinValue!.Value)
            {
                var diff = (double)(target.MinValue.Value - recipeAmount);
                score = 1 - diff / (double)target.MinValue.Value;
            }
            else if (recipeAmount > target.MaxValue!.Value)
            {
                var diff = (double)(recipeAmount - target.MaxValue.Value);
                score = 1 - diff / (double)target.MaxValue.Value;
            }
            else
            {
                score = 1;
            }

            score *= (double)target.Weight;

            return Math.Clamp(score, 0, 1);
        }

        private double CalculatePercentageNutrientScore(decimal recipeAmount,
                                                HealthGoalTarget target,
                                                decimal recipeCalories)
        {
            if (recipeCalories <= 0)
                return 0;

            double amount = (double)recipeAmount;
            double calories = (double)recipeCalories;

            double kcalPerGram = GetMacroCaloriesPerGram(target.NutrientId);
            if (kcalPerGram == 0)
                return 0;

            double kcal = amount * kcalPerGram;
            double pct = (kcal / calories) * 100;

            double minPct = (double)target.MinEnergyPct!;

            double maxPct = (double)target.MaxEnergyPct!;

            double score;

            if (pct < minPct)
            {
                double diff = minPct - pct;
                score = 1 - diff / minPct;
            }
            else if (pct > maxPct)
            {
                double diff = pct - maxPct;
                score = 1 - diff / maxPct;
            }
            else
            {
                score = 1;
            }

            score *= (double)target.Weight;

            return Math.Clamp(score, 0, 1);
        }


        private double CalculateTdeeFit(Recipe recipe, UserHealthMetric? metric)
        {
            if (metric == null)
                return 0;

            var tdee = metric.TDEE;

            double diff = Math.Abs((double)recipe.Calories - (double)tdee);
            double score = 1 - diff / (double)tdee;

            return Math.Clamp(score, 0, 1);
        }

        //public double CalculateBehaviorFit(Guid recipeId, UserBehaviorStats stats)
        //{
        //    if (!stats.Recipes.TryGetValue(recipeId, out var entry))
        //        return 0;

        //    double score =
        //        0.10 * Normalize(entry.Views, stats.MaxViews) +
        //        0.15 * Normalize(entry.SearchClicks, stats.MaxSearchClicks) +
        //        0.25 * Normalize(entry.Likes, stats.MaxLikes) +
        //        0.25 * Normalize(entry.Saves, stats.MaxSaves) +
        //        0.15 * Normalize(entry.Comments, stats.MaxComments) +
        //        0.10 * Normalize(entry.Ratings, stats.MaxRatings);

        //    return score;
        //}

        private double CalculatePopularityScore(Recipe recipe)
        {
            return
                0.4 * Normalize(recipe.ViewCount, 10000) +
                0.3 * Normalize(recipe.RatingCount, 5000) +
                0.3 * Normalize(recipe.AvgRating, 5);
        }

        private double CalculateFreshnessScore(DateTime createdAtUtc)
        {
            var days = (DateTime.UtcNow - createdAtUtc).TotalDays;
            double score = Math.Exp(-(days / 30));
            return Math.Clamp(score, 0, 1);
        }

        private double Normalize(double value, double max)
        {
            if (max <= 0) return 0;
            return Math.Clamp(value / max, 0, 1);
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
