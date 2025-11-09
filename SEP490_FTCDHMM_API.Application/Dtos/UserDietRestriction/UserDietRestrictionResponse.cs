using SEP490_FTCDHMM_API.Domain.ValueObjects;

namespace SEP490_FTCDHMM_API.Application.Dtos.UserDietRestriction
{
    public class UserDietRestrictionResponse
    {
        public Guid Id { get; set; }

        public string? IngredientName { get; set; }
        public string? IngredientCategoryName { get; set; }

        public RestrictionType Type { get; set; } = RestrictionType.Allergy;
        public string? Notes { get; set; }

        public DateTime? ExpiredAtUtc { get; set; }
    }
}
