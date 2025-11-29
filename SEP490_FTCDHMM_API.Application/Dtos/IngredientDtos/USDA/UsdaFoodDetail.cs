namespace SEP490_FTCDHMM_API.Application.Dtos.IngredientDtos.USDA
{
    public class UsdaFoodDetail
    {
        public int FdcId { get; set; }
        public string Description { get; set; } = string.Empty;
        public List<UsdaFoodNutrient> FoodNutrients { get; set; } = new();
    }
}
