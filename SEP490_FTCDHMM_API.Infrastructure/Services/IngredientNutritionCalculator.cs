using SEP490_FTCDHMM_API.Application.Dtos.IngredientDtos.Nutrient;
using SEP490_FTCDHMM_API.Application.Interfaces.SystemServices;
using SEP490_FTCDHMM_API.Domain.Interfaces;

namespace SEP490_FTCDHMM_API.Infrastructure.Services
{
    public class IngredientNutritionCalculator : IIngredientNutritionCalculator
    {
        private readonly INutrientIdProvider _idProvider;

        public IngredientNutritionCalculator(INutrientIdProvider idProvider)
        {
            _idProvider = idProvider;
        }

        public decimal CalculateCalories(IEnumerable<NutrientValueInput> nutrients)
        {
            decimal carbs = 0, protein = 0, fat = 0;

            foreach (var n in nutrients)
            {
                if (n.NutrientId == _idProvider.CarbohydrateId)
                    carbs = n.Median;

                if (n.NutrientId == _idProvider.ProteinId)
                    protein = n.Median;

                if (n.NutrientId == _idProvider.FatId)
                    fat = n.Median;
            }

            return (carbs * 4m) + (protein * 4m) + (fat * 9m);
        }
    }
}
