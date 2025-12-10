namespace SEP490_FTCDHMM_API.Domain.Entities
{
    public class CustomHealthGoal
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }

        public ICollection<HealthGoalTarget> Targets { get; set; } = null!;
        public AppUser User { get; set; } = null!;
    }
}
