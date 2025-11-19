using SEP490_FTCDHMM_API.Domain.ValueObjects;

namespace SEP490_FTCDHMM_API.Domain.Entities
{
    public class Recipe
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public string Description { get; set; } = string.Empty;
        public required Guid AuthorId { get; set; }
        public AppUser Author { get; set; } = null!;
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public required DifficultyValue Difficulty { get; set; }
        public int CookTime { get; set; }
        public DateTime UpdatedAtUtc { get; set; }
        public required int Ration { get; set; }
        public Guid? ImageId { get; set; }
        public Image? Image { get; set; }
        public decimal? Calories { get; set; }
        public bool IsDeleted { get; set; } = false;
        public double Rating { get; set; } = 0;

        public ICollection<CookingStep> CookingSteps { get; set; } = new List<CookingStep>();
        public ICollection<Label> Labels { get; set; } = new List<Label>();
        public ICollection<RecipeIngredient> RecipeIngredients { get; set; } = new List<RecipeIngredient>();
        public ICollection<RecipeNutritionAggregate> NutritionAggregates { get; set; } = new List<RecipeNutritionAggregate>();
        public ICollection<UserRecipeView> Views { get; set; } = new List<UserRecipeView>();
        public ICollection<UserFavoriteRecipe> FavoritedBy { get; set; } = new List<UserFavoriteRecipe>();
        public ICollection<UserSaveRecipe> SavedBy { get; set; } = new List<UserSaveRecipe>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<Rating> Ratings { get; set; } = new List<Rating>();
        public ICollection<RecipeUserTag> RecipeUserTags { get; set; } = new List<RecipeUserTag>();

    }
}
