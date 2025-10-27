namespace SEP490_FTCDHMM_API.Domain.Entities
{
    public class UserHealthGoal
    {
        public Guid UserId { get; set; }
        public Guid HealthGoalId { get; set; }
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

        public AppUser User { get; set; } = null!;
        public HealthGoal HealthGoal { get; set; } = null!;
    }

}
