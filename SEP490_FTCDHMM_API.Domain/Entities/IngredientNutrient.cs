namespace SEP490_FTCDHMM_API.Domain.Entities
{
    public class IngredientNutrient
    {
        public Guid IngredientId { get; set; }
        public Ingredient Ingredient { get; set; } = null!;
        public Guid NutrientId { get; set; }
        public Nutrient Nutrient { get; set; } = null!;

        public decimal? Min { get; set; }
        public decimal? Max { get; set; }
        public decimal Median { get; set; }
    }
}
