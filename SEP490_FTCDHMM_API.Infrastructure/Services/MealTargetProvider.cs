using SEP490_FTCDHMM_API.Application.Interfaces.SystemServices;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.Interfaces;
using SEP490_FTCDHMM_API.Domain.ValueObjects;

namespace SEP490_FTCDHMM_API.Infrastructure.Services
{
    public class MealTargetProvider : IMealTargetProvider
    {
        private readonly INutrientIdProvider _nutrientIdProvider;

        public MealTargetProvider(INutrientIdProvider nutrientIdProvider)
        {
            _nutrientIdProvider = nutrientIdProvider;
        }

        public MealTarget BuildMealTarget(
            double tdee,
            UserMealSlot slot,
            IReadOnlyList<NutrientTarget> nutrientTargets)
        {
            var targetCalories = (decimal)tdee;

            var normalizedTargets = nutrientTargets.Select(t =>
            {
                if (t.TargetType == NutrientTargetType.EnergyPercent)
                {
                    var kcalPerGram = GetMacroCaloriesPerGram(t.NutrientId);

                    var min = t.MinEnergyPct.HasValue
                        ? targetCalories * ((decimal)t.MinEnergyPct.Value / 100m) / kcalPerGram
                        : 0;

                    var max = t.MaxEnergyPct.HasValue
                        ? targetCalories * ((decimal)t.MaxEnergyPct.Value / 100m) / kcalPerGram
                        : decimal.MaxValue;

                    return new NutrientTarget(
                        t.NutrientId,
                        NutrientTargetType.Absolute,
                        min,
                        max,
                        null,
                        null,
                        t.Weight);
                }

                if (!IsMacro(t.NutrientId))
                {
                    var min = t.MinValue * slot.EnergyPercent;
                    var max = t.MaxValue * slot.EnergyPercent;

                    return new NutrientTarget(
                        t.NutrientId,
                        NutrientTargetType.Absolute,
                        min,
                        max,
                        null,
                        null,
                        t.Weight);
                }
                return t;
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

        private bool IsMacro(Guid nutrientId)
        {
            return nutrientId == _nutrientIdProvider.ProteinId
                || nutrientId == _nutrientIdProvider.CarbohydrateId
                || nutrientId == _nutrientIdProvider.FatId;
        }
    }
}
