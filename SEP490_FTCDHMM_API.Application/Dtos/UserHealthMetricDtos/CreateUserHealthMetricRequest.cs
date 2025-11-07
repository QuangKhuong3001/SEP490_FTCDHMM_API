namespace SEP490_FTCDHMM_API.Application.Dtos.UserHealthMetricDtos
{
    public class CreateUserHealthMetricRequest
    {
        public decimal WeightKg { get; set; }
        public decimal HeightCm { get; set; }
        public decimal? BodyFatPercent { get; set; }
        public decimal? MuscleMassKg { get; set; }
        public string? Notes { get; set; }
    }
}