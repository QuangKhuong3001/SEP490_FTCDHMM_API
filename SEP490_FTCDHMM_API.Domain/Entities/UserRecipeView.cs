namespace SEP490_FTCDHMM_API.Domain.Entities
{
    public class UserRecipeView
    {
        required public Guid UserId { get; set; }
        public AppUser User { get; set; } = null!;

        required public Guid RecipeId { get; set; }
        public Recipe Recipe { get; set; } = null!;
        public DateTime ViewedAtUtc { get; set; } = DateTime.UtcNow;
    }
}
