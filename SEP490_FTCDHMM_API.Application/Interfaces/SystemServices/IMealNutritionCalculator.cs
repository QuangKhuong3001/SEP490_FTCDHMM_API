using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos.Recommentdation;
using SEP490_FTCDHMM_API.Domain.ValueObjects;

namespace SEP490_FTCDHMM_API.Application.Interfaces.SystemServices
{
    public interface IMealNutritionCalculator
    {
        MealNutritionState Calculate(IEnumerable<RecipeScoringSnapshot> recipes);
    }
}
