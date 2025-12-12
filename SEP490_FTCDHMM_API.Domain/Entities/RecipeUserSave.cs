namespace SEP490_FTCDHMM_API.Domain.Entities
{
    public class RecipeUserSave
    {
        public Guid UserId { get; set; }
        public Guid RecipeId { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public AppUser User { get; set; } = null!;
        public Recipe Recipe { get; set; } = null!;
    }
}
