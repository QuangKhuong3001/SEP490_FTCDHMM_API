using SEP490_FTCDHMM_API.Domain.ValueObjects;

namespace SEP490_FTCDHMM_API.Domain.Entities
{
    public class UserHealthMetric
    {
        public Guid Id { get; set; }
        public decimal WeightKg { get; set; }
        public decimal HeightCm { get; set; }
        public decimal BMI { get; set; }
        public decimal? BodyFatPercent { get; set; }
        public decimal? MuscleMassKg { get; set; }
        public ActivityLevel ActivityLevel { get; set; } = ActivityLevel.Moderate;
        public decimal BMR { get; set; }
        public decimal TDEE { get; set; }
        public string? Notes { get; set; }

        public DateTime RecordedAt { get; set; } = DateTime.UtcNow;
        public Guid UserId { get; set; }
        public AppUser User { get; set; } = null!;
    }
}
