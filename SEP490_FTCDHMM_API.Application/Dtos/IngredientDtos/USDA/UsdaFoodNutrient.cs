namespace SEP490_FTCDHMM_API.Application.Dtos.IngredientDtos.USDA
{
    public class UsdaFoodNutrient
    {
        public UsdaNutrientInfo Nutrient { get; set; } = new();
        public decimal? Amount { get; set; }
    }
}
