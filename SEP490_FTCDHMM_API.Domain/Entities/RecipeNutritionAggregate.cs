namespace SEP490_FTCDHMM_API.Domain.Entities
{
    public class RecipeNutritionAggregate
    {
        public Guid RecipeId { get; set; }
        public Recipe Recipe { get; set; } = null!;

        public Guid NutrientId { get; set; }
        public Nutrient Nutrient { get; set; } = null!;

        public decimal Amount { get; set; }

        public decimal AmountPerServing { get; set; }

        public DateTime ComputedAtUtc { get; set; } = DateTime.UtcNow;
    }
}
