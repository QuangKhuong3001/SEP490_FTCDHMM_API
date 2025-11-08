namespace SEP490_FTCDHMM_API.Api.Dtos.UserHealthMetricDtos
{
    public class UserHealthMetricResponse
    {
        public Guid Id { get; set; }
        public decimal WeightKg { get; set; }
        public decimal HeightCm { get; set; }
        public decimal BMI { get; set; }
        public decimal? BodyFatPercent { get; set; }
        public decimal? MuscleMassKg { get; set; }
        public decimal BMR { get; set; }
        public decimal TDEE { get; set; }
        public DateTime RecordedAt { get; set; }
        public string? Notes { get; set; }
    }
}
