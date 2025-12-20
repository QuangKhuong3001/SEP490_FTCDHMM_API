using SEP490_FTCDHMM_API.Application.Dtos.NutrientDtos.NutrientTarget;

namespace SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos.Recommentdation
{
    public class RecommendationUserContext
    {
        public Guid UserId { get; set; }
        public double Tdee { get; set; }

        public List<NutrientTargetDto> Targets { get; set; } = new();

        public HashSet<Guid> RestrictedIngredientIds { get; set; } = new();
        public HashSet<Guid> RestrictedCategoryIds { get; set; } = new();

        public Dictionary<Guid, int> RatingByLabel { get; set; } = new();
        public Dictionary<Guid, int> ViewByLabel { get; set; } = new();
        public Dictionary<Guid, int> CommentByLabel { get; set; } = new();
        public Dictionary<Guid, int> SaveByLabel { get; set; } = new();

    }
}
