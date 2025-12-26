using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos.Recommentdation;

namespace SEP490_FTCDHMM_API.Application.Dtos.MealDtos
{
    public class MealAnalyzeResponse
    {
        public decimal TargetCalories { get; set; }
        public decimal CurrentCalories { get; set; }
        public decimal RemainingCalories { get; set; }
        public double EnergyCoveragePercent { get; set; }

        public Dictionary<Guid, decimal> TargetNutrients { get; set; } = new();
        public Dictionary<Guid, decimal> CurrentNutrients { get; set; } = new();
        public Dictionary<Guid, NutrientRangeResponse> RemainingNutrients { get; set; } = new();

        public List<RecipeRankResponse> Suggestions { get; set; } = new();
    }
}
