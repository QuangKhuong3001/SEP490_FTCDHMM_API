using Microsoft.Extensions.Options;
using SEP490_FTCDHMM_API.Application.Interfaces.SystemServices;
using SEP490_FTCDHMM_API.Domain.Enum;
using SEP490_FTCDHMM_API.Domain.Interfaces;
using SEP490_FTCDHMM_API.Domain.ValueObjects;
using SEP490_FTCDHMM_API.Infrastructure.ModelSettings;

namespace SEP490_FTCDHMM_API.Infrastructure.Services
{
    public class MealTargetProvider : IMealTargetProvider
    {
        private readonly MealDistributionSettings _mealDistribution;
        private readonly INutrientIdProvider _nutrientIdProvider;

        public MealTargetProvider(
            IOptions<MealDistributionSettings> mealDistribution,
            INutrientIdProvider nutrientIdProvider
            )
        {
            _mealDistribution = mealDistribution.Value;
            _nutrientIdProvider = nutrientIdProvider;
        }

        public MealTarget BuildMealTarget(
            double tdee,
            MealType mealType,
            IReadOnlyList<NutrientTarget> nutrientTargets)
        {
            var pct = mealType switch
            {
                MealType.Breakfast => _mealDistribution.Breakfast,
                MealType.Lunch => _mealDistribution.Lunch,
                _ => _mealDistribution.Dinner
            };

            var targetCalories = (decimal)(tdee * pct);

            var normalizedTargets = nutrientTargets.Select(t =>
            {
                if (t.TargetType != NutrientTargetType.EnergyPercent)
                    return t;

                var kcalPerGram = GetMacroCaloriesPerGram(t.NutrientId);

                var min = t.MinEnergyPct.HasValue
                    ? targetCalories * (decimal)t.MinEnergyPct.Value / kcalPerGram
                    : 0;

                var max = t.MaxEnergyPct.HasValue
                    ? targetCalories * (decimal)t.MaxEnergyPct.Value / kcalPerGram
                    : decimal.MaxValue;

                return new NutrientTarget(
                    t.NutrientId,
                    NutrientTargetType.Absolute,
                    min,
                    max,
                    null,
                    null,
                    t.Weight);
            }).ToList();

            return new MealTarget(targetCalories, normalizedTargets);
        }

        private decimal GetMacroCaloriesPerGram(Guid nutrientId)
        {
            if (nutrientId == _nutrientIdProvider.ProteinId) return 4m;
            if (nutrientId == _nutrientIdProvider.CarbohydrateId) return 4m;
            if (nutrientId == _nutrientIdProvider.FatId) return 9m;
            return 0;
        }
    }
}
