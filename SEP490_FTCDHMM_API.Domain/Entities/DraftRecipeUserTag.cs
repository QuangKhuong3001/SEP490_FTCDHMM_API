namespace SEP490_FTCDHMM_API.Domain.Entities
{
    public class DraftRecipeUserTag
    {
        public Guid DraftRecipeId { get; set; }
        public DraftRecipe DraftRecipe { get; set; } = null!;

        public Guid TaggedUserId { get; set; }
        public AppUser TaggedUser { get; set; } = null!;
    }
}
