namespace SEP490_FTCDHMM_API.Domain.Entities
{
    public class CustomHealthGoalTarget
    {
        public Guid Id { get; set; }
        public Guid CustomHealthGoalId { get; set; }
        public Guid NutrientId { get; set; }

        public decimal MinValue { get; set; }
        public decimal MaxValue { get; set; }

        public CustomHealthGoal CustomHealthGoal { get; set; } = null!;
        public Nutrient Nutrient { get; set; } = null!;
    }
}
