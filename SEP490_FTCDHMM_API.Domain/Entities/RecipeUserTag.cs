namespace SEP490_FTCDHMM_API.Domain.Entities
{
    public class RecipeUserTag
    {
        public Guid RecipeId { get; set; }
        public Recipe Recipe { get; set; } = null!;

        public Guid TaggedUserId { get; set; }
        public AppUser TaggedUser { get; set; } = null!;
    }
}
