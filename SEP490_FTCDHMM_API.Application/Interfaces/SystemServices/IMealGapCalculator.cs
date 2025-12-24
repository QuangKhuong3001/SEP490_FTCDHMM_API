using SEP490_FTCDHMM_API.Domain.ValueObjects;

namespace SEP490_FTCDHMM_API.Application.Interfaces.SystemServices
{
    public interface IMealGapCalculator
    {
        MealGap Calculate(MealTarget target, MealNutritionState current);
    }
}
