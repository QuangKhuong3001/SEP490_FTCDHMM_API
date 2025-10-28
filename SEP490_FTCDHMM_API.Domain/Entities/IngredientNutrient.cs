namespace SEP490_FTCDHMM_API.Domain.Entities
{
    public class IngredientNutrient
    {
        public Guid IngredientId { get; set; }
        public Ingredient Ingredient { get; set; } = null!;
        public Guid NutrientId { get; set; }
        public Nutrient Nutrient { get; set; } = null!;

        public decimal? MinPer100 { get; set; }
        public decimal? MaxPer100 { get; set; }
        public decimal MedianPer100g { get; set; }
    }
}
