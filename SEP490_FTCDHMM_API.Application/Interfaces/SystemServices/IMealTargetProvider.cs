using SEP490_FTCDHMM_API.Domain.Enum;
using SEP490_FTCDHMM_API.Domain.ValueObjects;

namespace SEP490_FTCDHMM_API.Application.Interfaces.SystemServices
{
    public interface IMealTargetProvider
    {
        MealTarget BuildMealTarget(double tdee, MealType mealType, IReadOnlyList<NutrientTarget> nutrientTargets);
    }
}
