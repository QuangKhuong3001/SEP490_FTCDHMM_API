using SEP490_FTCDHMM_API.Domain.ValueObjects;

namespace SEP490_FTCDHMM_API.Domain.Entities
{
    public class DraftRecipe
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; } = string.Empty;
        public DifficultyValue Difficulty { get; set; } = DifficultyValue.Easy;
        public int CookTime { get; set; } = 0;
        public int Ration { get; set; } = 0;
        public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
        public Guid? ImageId { get; set; }
        public Image? Image { get; set; }
        public Guid AuthorId { get; set; }
        public AppUser Author { get; set; } = null!;
        public ICollection<DraftCookingStep> DraftCookingSteps { get; set; } = new List<DraftCookingStep>();
        public ICollection<Label> Labels { get; set; } = new List<Label>();
        public ICollection<DraftRecipeIngredient> DraftRecipeIngredients { get; set; } = new List<DraftRecipeIngredient>();
        public ICollection<DraftRecipeUserTag> DraftRecipeUserTags { get; set; } = new List<DraftRecipeUserTag>();
    }
}
