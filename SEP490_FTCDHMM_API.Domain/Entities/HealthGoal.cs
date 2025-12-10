namespace SEP490_FTCDHMM_API.Domain.Entities
{
    public class HealthGoal
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string UpperName { get; set; } = string.Empty;
        public string? Description { get; set; }

        public ICollection<HealthGoalTarget> Targets { get; set; } = null!;
    }
}
