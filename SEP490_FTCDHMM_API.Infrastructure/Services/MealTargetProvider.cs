using Microsoft.Extensions.Options;
using SEP490_FTCDHMM_API.Application.Interfaces.SystemServices;
using SEP490_FTCDHMM_API.Domain.Enum;
using SEP490_FTCDHMM_API.Domain.ValueObjects;
using SEP490_FTCDHMM_API.Infrastructure.ModelSettings;

namespace SEP490_FTCDHMM_API.Infrastructure.Services
{
    public class MealTargetProvider : IMealTargetProvider
    {
        private readonly MealDistributionSettings _mealDistribution;

        public MealTargetProvider(IOptions<MealDistributionSettings> mealDistribution)
        {
            _mealDistribution = mealDistribution.Value;
        }

        public MealTarget BuildMealTarget(double tdee, MealType mealType, IReadOnlyList<NutrientTarget> nutrientTargets)
        {
            var pct = mealType switch
            {
                MealType.Breakfast => _mealDistribution.Breakfast,
                MealType.Lunch => _mealDistribution.Lunch,
                _ => _mealDistribution.Dinner
            };

            var targetCalories = (decimal)(tdee * pct);
            return new MealTarget(targetCalories, nutrientTargets);
        }
    }
}
