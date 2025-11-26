using SEP490_FTCDHMM_API.Domain.ValueObjects;

namespace SEP490_FTCDHMM_API.Domain.Entities
{
    public class UserHealthGoal
    {
        public Guid UserId { get; set; }
        public Guid? HealthGoalId { get; set; }
        public Guid? CustomHealthGoalId { get; set; }
        public DateTime? ExpiredAtUtc { get; set; }
        public HealthGoalType Type { get; set; } = HealthGoalType.CUSTOM;
        public AppUser User { get; set; } = null!;
        public HealthGoal? HealthGoal { get; set; }
        public CustomHealthGoal? CustomHealthGoal { get; set; }
    }

}
