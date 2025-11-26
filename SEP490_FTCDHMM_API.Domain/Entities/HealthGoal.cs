namespace SEP490_FTCDHMM_API.Domain.Entities
{
    public class HealthGoal
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }

        public ICollection<HealthGoalTarget> Targets { get; set; } = null!;
    }
}
