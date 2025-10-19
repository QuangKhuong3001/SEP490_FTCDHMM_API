namespace SEP490_FTCDHMM_API.Domain.Entities
{
    public class UserSaveRecipe
    {
        public required Guid UserId { get; set; }
        public required Guid RecipeId { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public AppUser User { get; set; } = null!;
        public Recipe Recipe { get; set; } = null!;
    }
}
