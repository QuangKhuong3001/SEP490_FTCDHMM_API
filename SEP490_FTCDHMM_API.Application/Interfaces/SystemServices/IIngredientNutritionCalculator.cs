using SEP490_FTCDHMM_API.Application.Dtos.IngredientDtos.Nutrient;

namespace SEP490_FTCDHMM_API.Application.Interfaces.SystemServices
{
    public interface IIngredientNutritionCalculator
    {
        decimal CalculateCalories(IEnumerable<NutrientValueInput> nutrients);
    }
}
