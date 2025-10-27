namespace SEP490_FTCDHMM_API.Domain.Entities
{
    public class HealthGoalTarget
    {
        public Guid Id { get; set; }
        public Guid HealthGoalId { get; set; }
        public Guid NutrientId { get; set; }

        public decimal MinValue { get; set; }
        public decimal MaxValue { get; set; }

        public HealthGoal HealthGoal { get; set; } = null!;
        public Nutrient Nutrient { get; set; } = null!;
    }
}
