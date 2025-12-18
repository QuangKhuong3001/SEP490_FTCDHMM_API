using Microsoft.Extensions.Options;
using SEP490_FTCDHMM_API.Application.Dtos.KMeans;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.Enum;
using SEP490_FTCDHMM_API.Domain.Interfaces;
using SEP490_FTCDHMM_API.Infrastructure.ModelSettings;

namespace SEP490_FTCDHMM_API.Infrastructure.Services
{
    public class ClusterRecipeScoringSystem : IClusterRecipeScoringSystem
    {
        private readonly FitScoreWeightsSettings _weights;
        private readonly MealDistributionSettings _mealDistribution;
        private readonly INutrientIdProvider _nutrientIdProvider;

        public ClusterRecipeScoringSystem(
            IOptions<FitScoreWeightsSettings> weights,
            IOptions<MealDistributionSettings> mealDistribution,
            INutrientIdProvider nutrientIdProvider)
        {
            _weights = weights.Value;
            _mealDistribution = mealDistribution.Value;
            _nutrientIdProvider = nutrientIdProvider;
        }

        public double CalculateClusterScore(
            Recipe recipe,
            ClusterProfile clusterProfile,
            TimeSpan currentTime)
        {
            var nutrientFit = CalculateMacroDistributionFit(recipe, clusterProfile);
            var tdeeFit = CalculateTdeeFit(recipe, clusterProfile.Tdee, currentTime);

            return
                (_weights.Nutrient * nutrientFit
                + _weights.Tdee * tdeeFit)
                /
                (_weights.Nutrient + _weights.Tdee);
        }

        private double CalculateMacroDistributionFit(Recipe recipe, ClusterProfile clusterProfile)
        {
            if (recipe.Calories <= 0)
                return 0;

            var carbPct = GetMacroEnergyPct(recipe, _nutrientIdProvider.CarbohydrateId);
            var proteinPct = GetMacroEnergyPct(recipe, _nutrientIdProvider.ProteinId);
            var fatPct = GetMacroEnergyPct(recipe, _nutrientIdProvider.FatId);

            var diff =
                Math.Abs(carbPct - clusterProfile.CarbPct) +
                Math.Abs(proteinPct - clusterProfile.ProteinPct) +
                Math.Abs(fatPct - clusterProfile.FatPct);

            return Math.Clamp(1 - diff, 0, 1);
        }

        private double CalculateTdeeFit(
            Recipe recipe,
            double avgTdee,
            TimeSpan currentTime)
        {
            if (avgTdee <= 0 || recipe.Ration <= 0)
                return 0;

            var meal = GetMealType(currentTime);

            double mealPct = meal switch
            {
                MealType.Breakfast => _mealDistribution.Breakfast,
                MealType.Lunch => _mealDistribution.Lunch,
                MealType.Dinner => _mealDistribution.Dinner,
                _ => 1.0 / 3.0
            };

            var targetCalories = avgTdee * mealPct;
            var caloriesPerServing = (double)recipe.Calories / recipe.Ration;

            var diff = Math.Abs(caloriesPerServing - targetCalories);
            return Math.Clamp(1 - diff / targetCalories, 0, 1);
        }

        private MealType GetMealType(TimeSpan time)
        {
            if (time >= new TimeSpan(5, 0, 0) && time < new TimeSpan(11, 0, 0))
                return MealType.Breakfast;

            if (time >= new TimeSpan(11, 0, 0) && time < new TimeSpan(16, 0, 0))
                return MealType.Lunch;

            return MealType.Dinner;
        }

        private double GetMacroEnergyPct(Recipe recipe, Guid nutrientId)
        {
            if (recipe.Calories <= 0)
                return 0;

            var aggregate = recipe.NutritionAggregates
                .FirstOrDefault(n => n.NutrientId == nutrientId);

            if (aggregate == null)
                return 0;

            var kcalPerGram = GetMacroCaloriesPerGram(nutrientId);
            if (kcalPerGram == 0)
                return 0;

            var kcal = (double)aggregate.AmountPerServing * kcalPerGram;
            var totalCalories = (double)recipe.Calories;

            return kcal / totalCalories;
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
