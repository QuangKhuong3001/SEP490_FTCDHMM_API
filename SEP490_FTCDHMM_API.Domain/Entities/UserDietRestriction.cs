using SEP490_FTCDHMM_API.Domain.ValueObjects;

namespace SEP490_FTCDHMM_API.Domain.Entities
{
    public class UserDietRestriction
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }

        public Guid? IngredientId { get; set; }
        public Guid? IngredientCategoryId { get; set; }

        public RestrictionType Type { get; set; } = RestrictionType.Allergy;
        public string? Notes { get; set; }

        public DateTime? ExpiredAtUtc { get; set; }

        public AppUser User { get; set; } = null!;
        public Ingredient? Ingredient { get; set; }
        public IngredientCategory? IngredientCategory { get; set; }
    }
}
