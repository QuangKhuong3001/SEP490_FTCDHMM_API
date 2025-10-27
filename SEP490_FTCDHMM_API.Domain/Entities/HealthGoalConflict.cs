namespace SEP490_FTCDHMM_API.Domain.Entities
{
    public class HealthGoalConflict
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid HealthGoalAId { get; set; }
        public HealthGoal HealthGoalA { get; set; } = null!;

        public Guid HealthGoalBId { get; set; }
        public HealthGoal HealthGoalB { get; set; } = null!;
    }
}
