namespace SEP490_FTCDHMM_API.Domain.Entities
{
    public class IngredientNutrient
    {
        public Guid IngredientId { get; set; }
        public Ingredient Ingredient { get; set; } = null!;
        public Guid NutrientId { get; set; }
        public Nutrient Nutrient { get; set; } = null!;

        public decimal? MinValue { get; set; }
        public decimal? MaxValue { get; set; }
        public decimal MedianValue { get; set; }
    }
}
