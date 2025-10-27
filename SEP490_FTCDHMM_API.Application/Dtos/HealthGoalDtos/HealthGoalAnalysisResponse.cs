namespace SEP490_FTCDHMM_API.Application.Dtos.HealthGoalDtos
{
    public class HealthGoalAnalysisResponse
    {
        public Guid RecipeId { get; set; }
        public string RecipeName { get; set; } = null!;
        public Guid HealthGoalId { get; set; }
        public string HealthGoalName { get; set; } = null!;
        public double MatchScore { get; set; }
        public decimal TotalCalories { get; set; }
        public Dictionary<string, decimal> Nutrients { get; set; } = new();
    }
}
