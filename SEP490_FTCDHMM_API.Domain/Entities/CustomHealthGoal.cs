namespace SEP490_FTCDHMM_API.Domain.Entities
{
    public class CustomHealthGoal
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }

        public ICollection<CustomHealthGoalTarget> Targets { get; set; } = new List<CustomHealthGoalTarget>();
        public AppUser User { get; set; } = null!;
    }
}
