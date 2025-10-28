namespace SEP490_FTCDHMM_API.Domain.Entities
{
    public class Ingredient
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public required string Name { get; set; }
        public string? Description { get; set; }
        public DateTime LastUpdatedUtc { get; set; } = DateTime.UtcNow;
        public decimal? Calories { get; set; }
        public int UsageFrequency { get; set; }
        public int SearchCount { get; set; }
        public double PopularityScore { get; set; }

        public Guid ImageId { get; set; }
        public required Image Image { get; set; }

        public ICollection<IngredientCategory> Categories { get; set; } = new List<IngredientCategory>();

        public ICollection<IngredientNutrient> IngredientNutrients { get; set; } = new List<IngredientNutrient>();
        public ICollection<Recipe> Recipes { get; set; } = new List<Recipe>();

    }
}
