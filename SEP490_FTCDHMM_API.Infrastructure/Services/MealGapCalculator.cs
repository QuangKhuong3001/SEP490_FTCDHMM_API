using SEP490_FTCDHMM_API.Application.Interfaces.SystemServices;
using SEP490_FTCDHMM_API.Domain.ValueObjects;

namespace SEP490_FTCDHMM_API.Infrastructure.Services
{
    public class MealGapCalculator : IMealGapCalculator
    {
        public MealGap Calculate(MealTarget target, MealNutritionState current)
        {
            var remainingCalories = target.TargetCalories - current.Calories;
            var remaining = new Dictionary<Guid, (decimal Min, decimal Max)>();

            foreach (var t in target.NutrientTargets)
            {
                current.Nutrients.TryGetValue(t.NutrientId, out var currentValue);

                var minRemain = t.MinValue - currentValue;
                var maxRemain = t.MaxValue - currentValue;

                remaining[t.NutrientId] = (
                    minRemain <= 0 ? 0 : minRemain,
                    maxRemain <= 0 ? 0 : maxRemain
                );
            }

            return new MealGap(remainingCalories, remaining);
        }
    }
}
