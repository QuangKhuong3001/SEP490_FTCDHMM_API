using Microsoft.Extensions.Options;
using SEP490_FTCDHMM_API.Application.Dtos.KMeans;
using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos.Recommentdation;
using SEP490_FTCDHMM_API.Domain.Interfaces;
using SEP490_FTCDHMM_API.Infrastructure.ModelSettings;

namespace SEP490_FTCDHMM_API.Infrastructure.Services
{
    public class ClusterRecipeScoringSystem : IClusterRecipeScoringSystem
    {
        private readonly MealDistributionSettings _mealDistribution;
        private readonly INutrientIdProvider _nutrientIdProvider;

        public ClusterRecipeScoringSystem(
            IOptions<MealDistributionSettings> mealDistribution,
            INutrientIdProvider nutrientIdProvider)
        {
            _mealDistribution = mealDistribution.Value;
            _nutrientIdProvider = nutrientIdProvider;
        }

        public double CalculateClusterScore(
            RecipeScoringSnapshot recipe,
            ClusterProfile cluster,
            TimeSpan currentTime)
        {
            var nutrientFit = CalculateMacroFit(recipe, cluster);
            var tdeeFit = CalculateTdeeFit(recipe, cluster.Tdee, currentTime);

            return 0.6 * nutrientFit + 0.4 * tdeeFit;
        }

        private double CalculateMacroFit(
            RecipeScoringSnapshot recipe,
            ClusterProfile cluster)
        {
            if (recipe.Calories <= 0 || recipe.Ration <= 0)
                return 0;

            var carbPct = GetMacroEnergyPct(recipe, _nutrientIdProvider.CarbohydrateId);
            var proteinPct = GetMacroEnergyPct(recipe, _nutrientIdProvider.ProteinId);
            var fatPct = GetMacroEnergyPct(recipe, _nutrientIdProvider.FatId);

            var diff =
                Math.Abs(carbPct - cluster.CarbPct) +
                Math.Abs(proteinPct - cluster.ProteinPct) +
                Math.Abs(fatPct - cluster.FatPct);

            return Math.Clamp(1 - diff / 3.0, 0, 1);
        }

        private double CalculateTdeeFit(
            RecipeScoringSnapshot recipe,
            double avgTdee,
            TimeSpan time)
        {
            if (avgTdee <= 0 || recipe.Ration <= 0)
                return 0;

            var mealPct = GetMealDistribution(time);
            var targetCalories = avgTdee * (double)mealPct;
            var perServing = (double)recipe.Calories / recipe.Ration;

            var diff = Math.Abs(perServing - targetCalories);
            return Math.Clamp(1 - diff / targetCalories, 0, 1);
        }

        private double GetMacroEnergyPct(
            RecipeScoringSnapshot recipe,
            Guid nutrientId)
        {
            var aggregate = recipe.NutritionAggregates
                .FirstOrDefault(n => n.NutrientId == nutrientId);

            if (aggregate == null || recipe.Calories <= 0)
                return 0;

            var kcal = (double)aggregate.AmountPerServing * GetMacroCaloriesPerGram(nutrientId);
            return kcal / (double)recipe.Calories;
        }

        private decimal GetMealDistribution(TimeSpan time)
        {
            if (time < new TimeSpan(11, 0, 0))
                return _mealDistribution.Breakfast;
            if (time < new TimeSpan(16, 0, 0))
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
